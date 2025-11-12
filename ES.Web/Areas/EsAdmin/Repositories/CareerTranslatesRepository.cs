using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Web.Areas.EsAdmin.Models;

namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class CareerTranslatesRepository
    {
        private readonly ApplicationDbContext _context;
        public CareerTranslatesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CareerTranslate?> GetCareerTranslateByIdAsync(int id)
        {
            return await _context.CareerTranslates.FindAsync(id);
        }

        public async Task<CareerTranslatesFormViewModel> InitializeCareerTranslatesFormViewModelAsync(int careerId, CareerTranslatesFormViewModel? model = null)
        {
            model ??= new CareerTranslatesFormViewModel(); // Initialize a new model if none is provided.

            model.Languages = await GetLanguagesAsync(careerId);
            return model;
        }

        public async Task<int> AddCareerTranslateAsync(CareerTranslate careerTranslate)
        {
            await _context.CareerTranslates.AddAsync(careerTranslate);
            await _context.SaveChangesAsync();
            return careerTranslate.Id;
        }

        public async Task<bool> DeleteTranslationAsync(int id)
        {
            _context.CareerTranslates.Remove(new CareerTranslate { Id = id });
            return await _context.SaveChangesAsync() > 0;
        }



        public async Task<IEnumerable<SelectListItem>> GetLanguagesAsync(int careerId)
        {
            // Get all language IDs that already have translations for the given career
            var translatedLanguageIds = await _context.CareerTranslates
                .Where(ct => ct.CareerId == careerId)
                .Select(ct => ct.LanguageId)
                .ToListAsync();

            // to know the default lang of the career and not showing it in the dropdown
            var category = await _context.Careers
                .Include(c => c.Language)
                .SingleOrDefaultAsync(c => c.Id == careerId);

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



        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
