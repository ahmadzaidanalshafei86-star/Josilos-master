using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Web.Areas.EsAdmin.Models;

namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class PageTranslatesRepository
    {
        private readonly ApplicationDbContext _context;
        public PageTranslatesRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PageTranslationFormViewModel> InitializePageTranslatesFormViewModelAsync(int pageId, PageTranslationFormViewModel? model = null)
        {
            model ??= new PageTranslationFormViewModel(); // Initialize a new model if none is provided.

            model.Languages = await GetLanguagesAsync(pageId);
            return model;
        }



        public async Task<PageTranslate> GetPageTranslateByIdAsync(int id)
        {
            var translate = await _context.PageTranslates.Where(pt => pt.Id == id)
                .Include(pt => pt.Page)
                .ThenInclude(pt => pt.Category)
                .FirstOrDefaultAsync();

            if (translate == null)
                throw new Exception(message: "Page not found");

            return translate;
        }

        public async Task<int> AddPageTranslateAsync(PageTranslate pageTranslate)
        {
            await _context.PageTranslates.AddAsync(pageTranslate);
            await _context.SaveChangesAsync();
            return pageTranslate.Id;
        }

        public async Task<bool> DeleteTranslationAsync(int id)
        {
            var translate = await GetPageTranslateByIdAsync(id);

            _context.PageTranslates.Remove(translate);
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<IEnumerable<SelectListItem>> GetLanguagesAsync(int pageId)
        {
            // Get all language IDs that already have translations for the given page
            var translatedLanguageIds = await _context.PageTranslates
                .Where(ct => ct.PageId == pageId)
                .Select(ct => ct.LanguageId)
                .ToListAsync();

            // to know the default lang of the page and not showing it in the dropdown
            var page = await _context.Pages
                .Include(c => c.Language)
                .SingleOrDefaultAsync(c => c.Id == pageId);

            if (page == null)
                throw new Exception(message: "page not found");

            return await _context.Languages
           .Where(l => l.Code != page.Language.Code && !translatedLanguageIds.Contains(l.Id))
          .Select(th => new SelectListItem
          {
              Value = th.Id.ToString(),
              Text = th.Name
          })
          .ToListAsync();
        }



        public void UpdatepageTranslate(PageTranslate pageTranslate)
        {
            _context.PageTranslates.Update(pageTranslate);
            _context.SaveChanges();
        }
    }
}
