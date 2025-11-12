using ES.Web.Areas.EsAdmin.Helpers;
using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;
using ES.Web.Areas.EsAdmin.Services;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class FormsController : Controller
    {
        private readonly FormRepository _formRepository;
        private readonly LanguagesRepository _languagesRepository;
        private readonly ILanguageService _languageService;

        public FormsController(FormRepository formRepository,
            ILanguageService languageService,
           LanguagesRepository languagesRepository)
        {
            _formRepository = formRepository;
            _languageService = languageService;
            _languagesRepository = languagesRepository;
        }

        [HttpGet]
        [Authorize(Permissions.Forms.Read)]
        public async Task<IActionResult> Index()
        {
            var forms = await _formRepository.GetFormsAsync();
            return View(forms);
        }

        [HttpGet]
        [Authorize(Permissions.Forms.Create)]
        public IActionResult Create()
        {
            FormViewModel model = new();
            return View("Form", model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FormViewModel model)
        {
            if (!User.HasClaim("Permission", Permissions.Forms.Create))
                return StatusCode(403);

            if (!ModelState.IsValid)
                return View("Form", model);

            var form = new Form
            {
                Title = model.Title,
                Description = model.Description,
                IsActive = model.IsActive,
                Email = model.Email,
                Fields = model.Fields.Select(f => new FormField
                {
                    FieldName = f.FieldName,
                    FieldHint = string.IsNullOrWhiteSpace(f.FieldHint) ? null : f.FieldHint, // Convert "" to null
                    FieldType = f.FieldType,
                    IsRequired = f.IsRequired,
                    IsPublished = f.IsPublished,
                    Order = f.Order,
                    Options = f.Options.Select(o => new FormOption
                    {
                        OptionText = o.OptionText,
                        Order = o.Order
                    }).ToList()
                }).ToList(),
                LanguageId = await _languagesRepository.GetLanguageByCode(await _languageService.GetDefaultDbCultureAsync()),
            };

            await _formRepository.AddAsync(form);

            return Json(new { success = true, message = "Form created successfully!", formId = form.Id });
        }

        [HttpGet]
        [Authorize(Permissions.Forms.Update)]
        public async Task<IActionResult> Edit(int id)
        {
            var form = await _formRepository.GetFullFormByIdAsync(id);
            if (form is null)
                return NotFound();

            var model = new FormViewModel
            {
                Id = form.Id,
                Title = form.Title,
                Description = form.Description,
                IsActive = form.IsActive,
                Email = form.Email,
                Fields = form.Fields.Select(f => new FormFieldViewModel
                {
                    Id = f.Id,
                    FieldName = f.FieldName,
                    FieldHint = f.FieldHint,
                    FieldType = f.FieldType,
                    IsRequired = f.IsRequired,
                    IsPublished = f.IsPublished,
                    Order = f.Order,
                    Options = f.Options.Select(o => new FormOptionViewModel
                    {
                        Id = o.Id,
                        OptionText = o.OptionText,
                        Order = o.Order
                    }).ToList()
                })
                .OrderBy(f => f.Order)
                .ToList()
            };

            return View("Form", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] FormViewModel model)
        {
            if (!User.HasClaim("Permission", Permissions.Forms.Update))
                return StatusCode(403);

            if (!ModelState.IsValid)
                return View("Form", model);

            var form = await _formRepository.GetFullFormByIdAsync(model.Id);

            if (form is null)
                return NotFound();

            form.Title = model.Title;
            form.Description = model.Description;
            form.Email = model.Email;

            // Step 1: Update, Add, or Remove Fields
            var existingFieldIds = form.Fields.Select(f => f.Id).ToList();
            var newFieldIds = model.Fields.Where(f => f.Id.HasValue).Select(f => f.Id!.Value).ToList();

            // Remove fields that are not in the new model
            var fieldsToRemove = form.Fields.Where(f => !newFieldIds.Contains(f.Id)).ToList();
            foreach (var field in fieldsToRemove)
            {
                form.Fields.Remove(field);
            }

            // Update existing fields and add new ones
            foreach (var fieldModel in model.Fields)
            {
                var existingField = form.Fields.FirstOrDefault(f => f.Id == fieldModel.Id);

                if (existingField != null)
                {
                    // Update existing field
                    existingField.FieldName = fieldModel.FieldName;
                    existingField.FieldHint = string.IsNullOrWhiteSpace(fieldModel.FieldHint) ? null : fieldModel.FieldHint;
                    existingField.FieldType = fieldModel.FieldType;
                    existingField.IsRequired = fieldModel.IsRequired;
                    existingField.IsPublished = fieldModel.IsPublished;
                    existingField.Order = fieldModel.Order;

                    // Update Options
                    var existingOptionIds = existingField.Options.Select(o => o.Id).ToList();
                    var newOptionIds = fieldModel.Options.Where(o => o.Id.HasValue).Select(o => o.Id!.Value).ToList();

                    // Remove options that are not in the new model
                    var optionsToRemove = existingField.Options.Where(o => !newOptionIds.Contains(o.Id)).ToList();
                    foreach (var option in optionsToRemove)
                    {
                        existingField.Options.Remove(option);
                    }

                    // Update existing options and add new ones
                    foreach (var optionModel in fieldModel.Options)
                    {
                        var existingOption = existingField.Options.FirstOrDefault(o => o.Id == optionModel.Id);
                        if (existingOption != null)
                        {
                            existingOption.OptionText = optionModel.OptionText;
                            existingOption.Order = optionModel.Order;
                        }
                        else
                        {
                            existingField.Options.Add(new FormOption
                            {
                                OptionText = optionModel.OptionText,
                                Order = optionModel.Order
                            });
                        }
                    }
                }
                else
                {
                    // Add new field
                    var newField = new FormField
                    {
                        FieldName = fieldModel.FieldName,
                        FieldType = fieldModel.FieldType,
                        FieldHint = string.IsNullOrWhiteSpace(fieldModel.FieldHint) ? null : fieldModel.FieldHint,
                        IsRequired = fieldModel.IsRequired,
                        IsPublished = fieldModel.IsPublished,
                        Order = fieldModel.Order,
                        Options = fieldModel.Options.Select(o => new FormOption
                        {
                            OptionText = o.OptionText,
                            Order = o.Order
                        }).ToList()
                    };

                    form.Fields.Add(newField);
                }
            }

            await _formRepository.UpdateAsync(form);

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.HasClaim("Permission", Permissions.Forms.Delete))
                return StatusCode(403);

            var form = await _formRepository.FindByIdAsync(id);

            if (form is null)
                return NotFound();

            await _formRepository.DeleteAsync(form);

            return StatusCode(200);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            if (!User.HasClaim("Permission", Permissions.Forms.Update))
                return StatusCode(403);

            var form = await _formRepository.FindByIdAsync(id);

            if (form is null)
                return StatusCode(402);

            form.IsActive = !form.IsActive;
            await _formRepository.UpdateStatusAsync(form);

            return StatusCode(200);
        }

        [HttpGet]
        public async Task<IActionResult> ExportSheet(int id)
        {
            var form = await _formRepository.GetFormWithResponsesAsync(id);

            if (form == null)
                return NotFound("Form not found");

            var responses = await _formRepository.GetResponsesByFormIdAsync(id);
            var excelFile = await ExcelExportHelper.GenerateExcel(form, responses);

            // Ensure the title is file-system safe
            string safeTitle = string.Join("_", form.Title.Split(Path.GetInvalidFileNameChars()));
            string fileName = $"{safeTitle}_Responses.xlsx";

            return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

    }
}

