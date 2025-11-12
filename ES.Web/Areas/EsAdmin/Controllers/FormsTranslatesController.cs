using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class FormsTranslatesController : Controller
    {
        private readonly FormRepository _formRepository;
        private readonly FormTranslatesRepository _formTranslatesRepository;

        public FormsTranslatesController(FormRepository formRepository,
            FormTranslatesRepository formTranslatesRepository)
        {
            _formRepository = formRepository;
            _formTranslatesRepository = formTranslatesRepository;
        }

        [HttpGet]
        [Authorize(Permissions.Forms.Read)]
        public async Task<IActionResult> Index(int formId)
        {
            var form = await _formRepository.GetFormWithTranslationsAsync(formId);

            if (form is null)
                return NotFound();

            FormTranslatesViewModel model = new()
            {
                FormId = formId,
                FormTitle = form.Title,
                FormDefaultLang = form.Language!.Code,
                CreatedDate = form.CreatedAt,
                PreEnteredTranslations = form.Translations?.ToList(),
            };
            return View(model);
        }

        [HttpGet]
        [Authorize(Permissions.Forms.Create)]
        public async Task<IActionResult> Create(int formId)
        {
            var model = await _formTranslatesRepository.InitializeFormTranslatesFormViewModelAsync(formId);
            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Forms.Create)]
        public async Task<IActionResult> Create(FormTranslationFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _formTranslatesRepository.InitializeFormTranslatesFormViewModelAsync(model.FormId, model);
                return View("Form", model);
            }
            await _formTranslatesRepository.SaveFormTranslationAsync(model);

            return RedirectToAction("Index", new { formId = model.FormId });
        }

        [HttpGet]
        [Authorize(Permissions.Forms.Update)]
        public async Task<IActionResult> Edit(int formId, int translationId)
        {
            var model = await _formTranslatesRepository.InitializeEditFormTranslatesFormViewModelAsync(formId, translationId);
            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Forms.Update)]
        public async Task<IActionResult> Edit(FormTranslationFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _formTranslatesRepository.InitializeEditFormTranslatesFormViewModelAsync(model.FormId, model.TranslationId);
                return View("Form", model);
            }

            await _formTranslatesRepository.UpdateFormTranslationAsync(model);

            return RedirectToAction("Index", new { formId = model.FormId });
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.HasClaim("Permission", Permissions.Forms.Delete))
                return StatusCode(403);

            var result = await _formTranslatesRepository.DeleteTranslationAsync(id); //returns true if deleted successfully
            if (result)
                return StatusCode(200);

            return BadRequest();

        }
    }
}
