using ES.Web.Helpers;
using ES.Web.Models;


namespace ES.Web.Services
{
    public class HomePageService
    {
        private readonly ApplicationDbContext _context;
        public HomePageService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<PageViewModel>> GetMainSliderAsync()
        {
            var languageId = await LanguageHelper.GetCurrentLanguageIdAsync(_context);

            var MainSliderCategory = await _context.Categories
                  .AsNoTracking()
                  .Include(c => c.CategoryTranslates)
                  .Include(c => c.PagesRelatedToThis)
                      .ThenInclude(p => p.PageTranslates)
                  .FirstOrDefaultAsync(c => c.Slug == "main-slider" && c.IsPublished);


            if (MainSliderCategory is null)
                return new List<PageViewModel>();

            var viewModel = MainSliderCategory.PagesRelatedToThis
                .Where(p => p.IsPublished)
                .OrderBy(p => p.Order)
                .Select(p =>
                {
                    var pageTranslate = p.PageTranslates?.FirstOrDefault(pt => pt.LanguageId == languageId);
                    return new PageViewModel
                    {
                        Slug = p.Slug,
                        ShortDescription = pageTranslate?.ShortDescription ?? p.ShortDescription,
                        CoverImageUrl = p.CoverImageUrl,
                        VideoUrl = p.VideoURL,
                    };
                })
                .ToList();

            return viewModel;
        }
    }
}
