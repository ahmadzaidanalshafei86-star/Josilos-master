using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Web.Areas.EsAdmin.Models;

namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class EcomCategoryTranslatesRepository
    {
        private readonly ApplicationDbContext _context;
        public EcomCategoryTranslatesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EcomCategoryTranslate> GetCategoryTranslateByIdAsync(int id)
        {
            var translate = await _context.EcomCategoriesTranslate.FindAsync(id);

            if (translate == null)
                throw new Exception(message: "Category not found");

            return translate;
        }

        public async Task<IEnumerable<SelectListItem>> GetLanguagesAsync(int categoryId)
        {
            // Get all language IDs that already have translations for the given category
            var translatedLanguageIds = await _context.EcomCategoriesTranslate
                .Where(ct => ct.EcomCategoryId == categoryId)
                .Select(ct => ct.LanguageId)
                .ToListAsync();

            // to know the default lang of the category and not showing it in the dropdown
            var category = await _context.EcomCategories
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

        public async Task<int> AddCategoryTranslateAsync(EcomCategoryTranslate EcomcategoryTranslate)
        {
            await _context.EcomCategoriesTranslate.AddAsync(EcomcategoryTranslate);
            await _context.SaveChangesAsync();
            return EcomcategoryTranslate.Id;
        }

        public async Task UpdateCategoryTrarnslation(EcomCategoryTranslate EcomCategoryTranslate)
        {
            _context.EcomCategoriesTranslate.Update(EcomCategoryTranslate);
            await _context.SaveChangesAsync();

        }

        public async Task<bool> DeleteTranslationAsync(int id)
        {
            var translate = _context.EcomCategoriesTranslate.Find(id);
            if (translate == null)
                throw new Exception(message: "Category not found");

            _context.EcomCategoriesTranslate.Remove(translate);
            await _context.SaveChangesAsync();

            return true;


        }

        public async Task<EcomCategoryTranslatesFormViewModel> InitializeCategoryTranslatesFormViewModelAsync(int categoryId, EcomCategoryTranslatesFormViewModel? model = null)
        {
            model ??= new EcomCategoryTranslatesFormViewModel(); // Initialize a new model if none is provided.

            model.Languages = await GetLanguagesAsync(categoryId);
            return model;
        }
    }
}
