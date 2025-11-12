using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Web.Areas.EsAdmin.Models;

namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class CategoriesTranslatesRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoriesTranslatesRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<CategoryTranslate> GetCategoryTranslateByIdAsync(int id)
        {
            var translate = await _context.CategoriesTranslate.FindAsync(id);

            if (translate == null)
                throw new Exception(message: "Category not found");

            return translate;

        }

        public async Task<IEnumerable<SelectListItem>> GetLanguagesAsync(int categoryId)
        {

            // Get all language IDs that already have translations for the given category
            var translatedLanguageIds = await _context.CategoriesTranslate
                .Where(ct => ct.CategoryId == categoryId)
                .Select(ct => ct.LanguageId)
                .ToListAsync();

            // to know the default lang of the category and not showing it in the dropdown
            var category = await _context.Categories
                .Include(c => c.Language)
                .SingleOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
                throw new Exception(message: "category not found");

            return await _context.Languages
           .Where(l => l.Code != category.Language.Code && !translatedLanguageIds.Contains(l.Id))
          .Select(th => new SelectListItem
          {
              Value = th.Id.ToString(),
              Text = th.Name
          })
          .ToListAsync();
        }

        public async Task<CategoryTranslationFormViewModel> InitializeCategoryTranslatesFormViewModelAsync(int categoryId, CategoryTranslationFormViewModel? model = null)
        {
            model ??= new CategoryTranslationFormViewModel(); // Initialize a new model if none is provided.

            model.Languages = await GetLanguagesAsync(categoryId);
            return model;
        }

        public async Task<int> AddCategoryTranslateAsync(CategoryTranslate categoryTranslate)
        {
            await _context.CategoriesTranslate.AddAsync(categoryTranslate);
            await _context.SaveChangesAsync();
            return categoryTranslate.Id;
        }


        public void UpdateCategory(CategoryTranslate categoryTranslate)
        {
            _context.CategoriesTranslate.Update(categoryTranslate);
            _context.SaveChanges();
        }

        public async Task<bool> DeleteTranslationAsync(int id)
        {
            var translate = await GetCategoryTranslateByIdAsync(id);

            _context.CategoriesTranslate.Remove(translate);
            await _context.SaveChangesAsync();

            return true;

        }
    }

}
