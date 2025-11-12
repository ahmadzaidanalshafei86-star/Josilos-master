using ES.Web.Helpers;
using ES.Web.Models;

namespace ES.Web.Services
{
    public class TopService
    {
        private readonly ApplicationDbContext _context;
        public TopService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<TopViewModel> GetTopAsync()
        {
            var languageId = await LanguageHelper.GetCurrentLanguageIdAsync(_context);

            var TopCategory = await _context.Categories
                .AsNoTracking()
                  .Include(c => c.PagesRelatedToThis)
                      .ThenInclude(p => p.PageTranslates)
                .FirstOrDefaultAsync(c => c.Slug == "top" && c.IsPublished);

            if (TopCategory == null)
                return new TopViewModel();

            var viewModel = new TopViewModel
            {
                Pages = TopCategory.PagesRelatedToThis
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
