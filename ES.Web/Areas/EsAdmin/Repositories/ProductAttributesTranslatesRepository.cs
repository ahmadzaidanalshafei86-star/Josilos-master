using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Web.Areas.EsAdmin.Models;

namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class ProductAttributesTranslatesRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductAttributesRepository _productAttributesRepository;

        public ProductAttributesTranslatesRepository(ApplicationDbContext context,
            ProductAttributesRepository productAttributesRepository)
        {
            _context = context;
            _productAttributesRepository = productAttributesRepository;
        }

        public async Task AddProductAttributeTranslateAsync(ProductAttributeTranslateFormModel model)
        {
            ProductAttributeTranslation productAttributeTranslation = new()
            {
                ProductAttributeId = model.AttributeId,
                LanguageId = model.LanguageId!.Value,
                TranslatedName = model.TranslatedName,
            };

            await _context.ProductAttributeTranslations.AddAsync(productAttributeTranslation);

            if (model.Values?.Any() == true && model.LanguageId.HasValue)
            {
                var translations = model.Values.Select(value => new ProductAttributeValueTranslation
                {
                    ProductAttributeValueId = value.ValueId,
                    LanguageId = model.LanguageId.Value,
                    TranslatedValue = value.TranslatedText
                }).ToList();

                await _context.ProductAttributeValueTranslations.AddRangeAsync(translations);
            }

            await _context.SaveChangesAsync();
        }


        public async Task<ProductAttributeTranslateFormModel> InitializeTranslatesFormViewModelAsync(int attirbuteId, ProductAttributeTranslateFormModel? model = null)
        {
            model ??= new ProductAttributeTranslateFormModel();

            var productAttribute = await _productAttributesRepository.GetAttibuteWithValuesByIdAsync(attirbuteId);

            model.AttributeId = attirbuteId;
            model.Name = productAttribute!.Name;

            model.Values = productAttribute.Values.Select(v => new ProductAttributeValueTranslateModel
            {
                ValueId = v.Id,
                OriginalText = v.Value,
                TranslatedText = model.Values.FirstOrDefault(vt => vt.ValueId == v.Id)?.TranslatedText ?? "",
                Order = v.Order,
            })
            .OrderBy(v => v.Order)
            .ToList();

            model.Languages = await GetLanguagesAsync(attirbuteId);

            return model;
        }
        public async Task<ProductAttributeTranslateFormModel> InitializeTranslatesEditFormViewModelAsync(int attributeId, int translationId)
        {
            var attribute = await _context.ProductAttributes
                .Include(a => a.Translations)
                .Include(a => a.Values)
                    .ThenInclude(v => v.Translations)
                .FirstOrDefaultAsync(a => a.Id == attributeId);

            if (attribute == null)
                throw new Exception("Product Attribute not found.");

            var translation = attribute.Translations.FirstOrDefault(t => t.Id == translationId);
            var languageId = translation?.LanguageId;

            var model = new ProductAttributeTranslateFormModel
            {
                TranslationId = translationId,
                LanguageId = languageId,
                AttributeId = attributeId,
                Name = attribute.Name,
                TranslatedName = translation?.TranslatedName ?? string.Empty,
                Values = attribute.Values.Select(v => new ProductAttributeValueTranslateModel
                {
                    ValueId = v.Id,
                    OriginalText = v.Value,
                    TranslatedText = v.Translations.FirstOrDefault(t => t.LanguageId == languageId)?.TranslatedValue ?? string.Empty,
                    Order = v.Order
                }).ToList()
            };

            return model;
        }
        public async Task UpdateTranslationsAsync(ProductAttributeTranslateFormModel model)
        {
            var productAttributeTranslation = await _context.ProductAttributeTranslations
                .FirstOrDefaultAsync(t => t.Id == model.TranslationId);

            if (productAttributeTranslation != null)
                productAttributeTranslation.TranslatedName = model.TranslatedName;

            if (model.Values?.Any() == true && model.LanguageId.HasValue)
            {
                foreach (var value in model.Values)
                {
                    var OldValueTranslate = await _context.ProductAttributeValueTranslations
                        .FirstOrDefaultAsync(t =>
                            t.ProductAttributeValueId == value.ValueId &&
                            t.LanguageId == model.LanguageId.Value);


                    OldValueTranslate!.TranslatedValue = value.TranslatedText;
                }
            }

            await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteTranslationAsync(int id)
        {
            var translation = await _context.ProductAttributeTranslations.FindAsync(id);
            if (translation is null)
                return false;

            int languageId = translation.LanguageId;
            int productAttributeId = translation.ProductAttributeId;

            // Find all ProductAttributeValues linked to this ProductAttribute
            var attributeValues = await _context.ProductAttributeValues
                .Where(v => v.ProductAttributeId == productAttributeId)
                .ToListAsync();

            // Get all related ProductAttributeValueTranslations with the same LanguageId
            var valueTranslations = await _context.ProductAttributeValueTranslations
                .Where(t => attributeValues.Select(v => v.Id).Contains(t.ProductAttributeValueId) &&
                            t.LanguageId == languageId)
                .ToListAsync();

            // Remove related ProductAttributeValueTranslations first
            _context.ProductAttributeValueTranslations.RemoveRange(valueTranslations);

            // Remove the ProductAttributeTranslation
            _context.ProductAttributeTranslations.Remove(translation);

            await _context.SaveChangesAsync();
            return true;
        }



        private async Task<IEnumerable<SelectListItem>> GetLanguagesAsync(int attributeId)
        {
            // Get all language IDs that already have translations for the given form
            var translatedLanguageIds = await _context.ProductAttributeTranslations
                .Where(at => at.ProductAttributeId == attributeId)
                .Select(ct => ct.LanguageId)
                .ToListAsync();

            // to know the default lang of the attribute and not showing it in the dropdown
            var attribute = await _context.ProductAttributes
                .Include(c => c.Language)
                .SingleOrDefaultAsync(c => c.Id == attributeId);

            if (attribute == null)
                throw new Exception(message: "form not found");

            return await _context.Languages
           .Where(l => l.Code != attribute.Language.Code && !translatedLanguageIds.Contains(l.Id))
          .Select(th => new SelectListItem
          {
              Value = th.Id.ToString(),
              Text = th.Name
          })
          .ToListAsync();
        }


    }
}
