using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Web.Areas.EsAdmin.Models;

namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class ProductTranslatesRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductTranslatesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SelectListItem>> GetLanguagesAsync(int productId)
        {
            // Get all language IDs that already have translations for the given product
            var translatedLanguageIds = await _context.ProductTranslates
                .Where(ct => ct.ProductId == productId)
                .Select(ct => ct.LanguageId)
                .ToListAsync();

            // to know the default lang of the product and not showing it in the dropdown
            var product = await _context.Products
                .Include(c => c.Language)
                .SingleOrDefaultAsync(c => c.Id == productId);

            if (product == null)
                throw new Exception(message: "product not found");

            return await _context.Languages
           .Where(l => l.Code != product.Language!.Code && !translatedLanguageIds.Contains(l.Id))
          .Select(th => new SelectListItem
          {
              Value = th.Id.ToString(),
              Text = th.Name
          })
          .ToListAsync();
        }

        public async Task<ProductTranslate?> GetProductTranslateByIdAsync(int id)
        {
            return await _context.ProductTranslates.FindAsync(id);
        }

        public async Task<ProductTranslateFormViewModel> InitializeProductTranslatesFormViewModelAsync(int productId, ProductTranslateFormViewModel? model = null)
        {
            model ??= new ProductTranslateFormViewModel(); // Initialize a new model if none is provided.

            model.Languages = await GetLanguagesAsync(productId);
            return model;
        }

        public async Task AddProductTranslateAsync(ProductTranslate ProductTranslate)
        {
            await _context.ProductTranslates.AddAsync(ProductTranslate);
        }

        public async Task AddProductTabTranslateAsync(ProductTabTranslation productTabTranslation)
        {
            await _context.ProductTabTranslations.AddAsync(productTabTranslation);
        }
        public async Task<IList<ProductTabViewModel>> GetTabTranslationsAsync(int productId, int languageId)
        {
            return await _context.ProductTabs
                .Where(pt => pt.ProductId == productId)
                .OrderBy(pt => pt.Order)
                .Select(pt => new ProductTabViewModel
                {
                    Id = pt.Id,
                    Title = _context.ProductTabTranslations
                                .Where(tt => tt.ProductTabId == pt.Id && tt.LanguageId == languageId)
                                .Select(tt => tt.TranslatedTitle)
                                .FirstOrDefault() ?? pt.Title,
                    Content = _context.ProductTabTranslations
                                .Where(tt => tt.ProductTabId == pt.Id && tt.LanguageId == languageId)
                                .Select(tt => tt.TranslatedContent)
                                .FirstOrDefault() ?? pt.Content
                })
                .ToListAsync();
        }


        public async Task UpdatProductTabTranslationsAsync(int productId, int languageId, IList<ProductTabViewModel> tabs)
        {
            foreach (var tabVm in tabs)
            {
                // Try to find existing translation
                var existingTranslation = await _context.ProductTabTranslations.FirstOrDefaultAsync(t =>
                    t.ProductTabId == tabVm.Id &&
                    t.LanguageId == languageId);

                if (existingTranslation != null)
                {
                    // Update existing translation
                    existingTranslation.TranslatedTitle = tabVm.Title;
                    existingTranslation.TranslatedContent = tabVm.Content;
                }
                else
                {
                    // Fallback: try to find the tab to determine the correct order
                    var productTab = await _context.ProductTabs.FirstOrDefaultAsync(pt => pt.Id == tabVm.Id);

                    if (productTab != null)
                    {
                        var newTranslation = new ProductTabTranslation
                        {
                            ProductId = productId,
                            ProductTabId = productTab.Id,
                            LanguageId = languageId,
                            TranslatedTitle = tabVm.Title,
                            TranslatedContent = tabVm.Content,
                            Order = productTab.Order
                        };

                        await _context.ProductTabTranslations.AddAsync(newTranslation);
                    }
                }
            }
        }


        public async Task<bool> DeleteTranslation(int translationId, int productId, int languageId)
        {
            var translation = await _context.ProductTranslates.FindAsync(translationId);

            if (translation is not null)
                _context.ProductTranslates.Remove(translation);

            var productTabTranslates = await _context.ProductTabTranslations
                .Where(ptt => ptt.ProductId == productId && ptt.LanguageId == languageId)
                .ToListAsync();

            if (productTabTranslates.Any())
                _context.ProductTabTranslations.RemoveRange(productTabTranslates);

            var changes = await _context.SaveChangesAsync();

            return changes > 0;
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


    }
}
