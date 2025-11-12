using ES.Web.Helpers;
using ES.Web.Models;

namespace ES.Web.Services
{
    public class FooterService
    {
        private readonly ApplicationDbContext _context;
        public FooterService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<FooterViewModel> GetFooterAsync()
        {
            var languageId = await LanguageHelper.GetCurrentLanguageIdAsync(_context);

            var footerCategory = await _context.Categories
                .AsNoTracking()
                  .Include(c => c.PagesRelatedToThis)
                      .ThenInclude(p => p.PageTranslates)
                .FirstOrDefaultAsync(c => c.Slug == "footer" && c.IsPublished);

            if (footerCategory == null)
                return new FooterViewModel();

            var viewModel = new FooterViewModel
            {
                Pages = footerCategory.PagesRelatedToThis
                    .Where(p => p.IsPublished)
                    .OrderBy(p => p.Order)
                    .Select(p =>
                    {
                        var pageTranslate = p.PageTranslates?.FirstOrDefault(pt => pt.LanguageId == languageId);
                        return new PageViewModel
                        {
                            Title = pageTranslate?.Title ?? p.Title,
                            ShortDescription = pageTranslate?.ShortDescription ?? p.ShortDescription,
                        };
                    })
                    .ToList(),
            };

            return viewModel;
        }


    }
}
