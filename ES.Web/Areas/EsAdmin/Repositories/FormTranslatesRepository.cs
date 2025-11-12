using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Web.Areas.EsAdmin.Models;

namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class FormTranslatesRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly FormRepository _formRepository;
        public FormTranslatesRepository(ApplicationDbContext context, FormRepository formRepository)
        {
            _context = context;
            _formRepository = formRepository;
        }


        public async Task<FormTranslation> GetFormTranslateByIdAsync(int id)
        {
            return await _context.FormTranslations.FindAsync(id);
        }


        //Used For Create
        public async Task<FormTranslationFormViewModel> InitializeFormTranslatesFormViewModelAsync(int FormId, FormTranslationFormViewModel? model = null)
        {
            model ??= new FormTranslationFormViewModel(); // Initialize if null

            var form = await _formRepository.GetFullFormByIdAsync(FormId);
            if (form == null)
                throw new Exception("Form not found");

            model.FormId = FormId;
            model.Title = form.Title;
            model.Description = form.Description;

            // Map fields and options
            model.Fields = form.Fields.Select(f => new FieldTranslationViewModel
            {
                FieldId = f.Id,
                Order = f.Order,
                OriginalText = f.FieldName,
                TranslatedFieldHint = f.FieldHint,
                TranslatedText = model.Fields?.FirstOrDefault(ft => ft.FieldId == f.Id)?.TranslatedText ?? "", // Preserve input if reloading
                Options = f.Options.Select(o => new OptionTranslationViewModel
                {
                    OptionId = o.Id,
                    OriginalText = o.OptionText,
                    TranslatedText = model.Fields?
                        .FirstOrDefault(ft => ft.FieldId == f.Id)?
                        .Options.FirstOrDefault(ot => ot.OptionId == o.Id)?
                        .TranslatedText ?? ""
                }).ToList()
            }).OrderBy(f => f.Order)
                .ToList();


            // Load languages
            model.Languages = await GetLanguagesAsync(FormId);

            return model;
        }

        public async Task SaveFormTranslationAsync(FormTranslationFormViewModel model)
        {
            // Save form translation
            var translation = new FormTranslation
            {
                FormId = model.FormId,
                LanguageId = model.LanguageId!.Value,
                TranslatedTitle = model.Title,
                TranslatedDescription = model.Description
            };
            _context.FormTranslations.Add(translation);
            await _context.SaveChangesAsync();

            // Save field and option translations
            foreach (var field in model.Fields)
            {
                var fieldTranslation = new FieldTranslation
                {
                    FieldId = field.FieldId,
                    LanguageId = model.LanguageId.Value,
                    TranslatedText = field.TranslatedText,
                    TranslatedFieldHint = string.IsNullOrWhiteSpace(field.TranslatedFieldHint) ? null : field.TranslatedFieldHint,
                };
                _context.FieldTranslations.Add(fieldTranslation);

                // Only add options if they exist and have valid OptionId
                if (field.Options != null && field.Options.Any())
                {
                    foreach (var option in field.Options)
                    {
                        if (option.OptionId != null)  // Ensure OptionId is valid before adding
                        {
                            var optionTranslation = new OptionTranslation
                            {
                                OptionId = option.OptionId, // Must be a valid FK reference
                                LanguageId = model.LanguageId.Value,
                                TranslatedText = option.TranslatedText
                            };
                            _context.OptionTranslations.Add(optionTranslation);
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();
        }


        public async Task<bool> DeleteTranslationAsync(int id)
        {
            var translate = await _context.FormTranslations
                .Include(ft => ft.Form) // Include Form to access FormId
                .FirstOrDefaultAsync(ft => ft.Id == id);

            if (translate == null)
                return false;

            int formId = translate.FormId;

            // Delete all related FieldTranslations
            var fieldTranslations = await _context.FieldTranslations
                .Where(ft => ft.Field.FormId == formId)
                .ToListAsync();
            _context.FieldTranslations.RemoveRange(fieldTranslations);

            // Delete all related OptionTranslations
            var optionTranslations = await _context.OptionTranslations
                .Where(ot => ot.Option.Field.FormId == formId)
                .ToListAsync();
            _context.OptionTranslations.RemoveRange(optionTranslations);

            // Delete the FormTranslation
            _context.FormTranslations.Remove(translate);

            // Save changes
            await _context.SaveChangesAsync();

            return true;
        }

        //Used For edit
        public async Task<FormTranslationFormViewModel> InitializeEditFormTranslatesFormViewModelAsync(int formId, int translationId)
        {
            var form = await _formRepository.GetFullFormByIdAsync(formId);
            if (form == null)
                throw new Exception("Form not found");

            var translation = await _context.FormTranslations
                .Include(ft => ft.Language)
                .FirstOrDefaultAsync(ft => ft.Id == translationId && ft.FormId == formId);

            if (translation == null)
                throw new Exception("Translation not found");

            var model = new FormTranslationFormViewModel
            {
                TranslationId = translationId,
                FormId = formId,
                Title = translation.TranslatedTitle,
                Description = translation.TranslatedDescription,
                Fields = form.Fields.Select(f => new FieldTranslationViewModel
                {
                    FieldId = f.Id,
                    Order = f.Order,
                    OriginalText = f.FieldName,
                    TranslatedText = _context.FieldTranslations
                        .Where(ft => ft.FieldId == f.Id && ft.LanguageId == translation.LanguageId)
                        .Select(ft => ft.TranslatedText)
                        .FirstOrDefault() ?? "",
                    TranslatedFieldHint = _context.FieldTranslations
                        .Where(ft => ft.FieldId == f.Id && ft.LanguageId == translation.LanguageId)
                        .Select(ft => ft.TranslatedFieldHint)
                        .FirstOrDefault() ?? "",
                    Options = f.Options.Select(o => new OptionTranslationViewModel
                    {
                        OptionId = o.Id,
                        OriginalText = o.OptionText,
                        TranslatedText = _context.OptionTranslations
                            .Where(ot => ot.OptionId == o.Id && ot.LanguageId == translation.LanguageId)
                            .Select(ot => ot.TranslatedText)
                            .FirstOrDefault() ?? ""
                    }).ToList()
                }).OrderBy(f => f.Order).ToList(),
            };

            return model;
        }


        public async Task UpdateFormTranslationAsync(FormTranslationFormViewModel model)
        {
            var translation = await _context.FormTranslations
                .FirstOrDefaultAsync(ft => ft.Id == model.TranslationId && ft.FormId == model.FormId);

            if (translation == null)
                throw new Exception("Translation not found");

            // Update title and description
            translation.TranslatedTitle = model.Title;
            translation.TranslatedDescription = model.Description;

            // Update field translations
            foreach (var field in model.Fields)
            {
                var fieldTranslation = await _context.FieldTranslations
                    .FirstOrDefaultAsync(ft => ft.FieldId == field.FieldId && ft.LanguageId == translation.LanguageId);

                if (fieldTranslation != null)
                {
                    fieldTranslation.TranslatedText = field.TranslatedText;
                    fieldTranslation.TranslatedFieldHint = string.IsNullOrWhiteSpace(field.TranslatedFieldHint) ? null : field.TranslatedFieldHint;
                }
                else
                {
                    // If translation doesn't exist, create a new one
                    _context.FieldTranslations.Add(new FieldTranslation
                    {
                        FieldId = field.FieldId,
                        LanguageId = translation.LanguageId,
                        TranslatedText = field.TranslatedText,
                        TranslatedFieldHint = string.IsNullOrWhiteSpace(field.TranslatedFieldHint) ? null : field.TranslatedFieldHint,
                    });
                }

                // Update option translations
                foreach (var option in field.Options)
                {
                    var optionTranslation = await _context.OptionTranslations
                        .FirstOrDefaultAsync(ot => ot.OptionId == option.OptionId && ot.LanguageId == translation.LanguageId);

                    if (optionTranslation != null)
                    {
                        optionTranslation.TranslatedText = option.TranslatedText;
                    }
                    else
                    {
                        // If translation doesn't exist, create a new one
                        _context.OptionTranslations.Add(new OptionTranslation
                        {
                            OptionId = option.OptionId,
                            LanguageId = translation.LanguageId,
                            TranslatedText = option.TranslatedText
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
        }


        private async Task<IEnumerable<SelectListItem>> GetLanguagesAsync(int formId)
        {
            // Get all language IDs that already have translations for the given form
            var translatedLanguageIds = await _context.FormTranslations
                .Where(ct => ct.FormId == formId)
                .Select(ct => ct.LanguageId)
                .ToListAsync();

            // to know the default lang of the form and not showing it in the dropdown
            var form = await _context.Forms
                .Include(c => c.Language)
                .SingleOrDefaultAsync(c => c.Id == formId);

            if (form == null)
                throw new Exception(message: "form not found");

            return await _context.Languages
           .Where(l => l.Code != form.Language.Code && !translatedLanguageIds.Contains(l.Id))
          .Select(th => new SelectListItem
          {
              Value = th.Id.ToString(),
              Text = th.Name
          })
          .ToListAsync();
        }


    }
}
