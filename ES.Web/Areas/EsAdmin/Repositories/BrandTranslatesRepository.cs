using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Web.Areas.EsAdmin.Models;

namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class BrandTranslatesRepository
    {
        private readonly ApplicationDbContext _context;
        public BrandTranslatesRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<BrandTranslate> GetBrandTranslateByIdAsync(int id)
        {
            return await _context.BrandTranslates.FindAsync(id)
                ?? throw new Exception(message: "Brand not found");
        }

        public async Task<BrandTranslateFormViewModel> InitializeBrandTranslatesFormViewModelAsync(int BrandId, BrandTranslateFormViewModel? model = null)
        {

            model ??= new BrandTranslateFormViewModel(); // Initialize a new model if none is provided.

            model.Languages = await GetLanguagesAsync(BrandId);
            return model;
        }
        public async Task<int> AddBrandTranslateAsync(BrandTranslate BrandTranslate)
        {
            await _context.BrandTranslates.AddAsync(BrandTranslate);
            await _context.SaveChangesAsync();
            return BrandTranslate.Id;
        }

        public async Task DeleteTranslationAsync(int id)
        {
            _context.BrandTranslates.Remove(await GetBrandTranslateByIdAsync(id));
            await _context.SaveChangesAsync();
        }


        public async Task UpdateTranslate(BrandTranslate BrandTranslate)
        {
            _context.BrandTranslates.Update(BrandTranslate);
            await _context.SaveChangesAsync();
        }

        private async Task<IEnumerable<SelectListItem>> GetLanguagesAsync(int BrandId)
        {
            // Get all language IDs that already have translations for the given Brand
            var translatedLanguageIds = await _context.BrandTranslates
                .Where(ct => ct.BrandId == BrandId)
                .Select(ct => ct.LanguageId)
                .ToListAsync();

            // to know the default lang of the brand and not showing it in the dropdown
            var brand = await _context.Brands
                .Include(c => c.Language)
                .SingleOrDefaultAsync(c => c.Id == BrandId);

            if (brand == null)
                throw new Exception(message: "Brand not found");

            return await _context.Languages
           .Where(l => l.Code != brand.Language.Code && !translatedLanguageIds.Contains(l.Id))
          .Select(th => new SelectListItem
          {
              Value = th.Id.ToString(),
              Text = th.Name
          })
          .ToListAsync();
        }
    }
}
