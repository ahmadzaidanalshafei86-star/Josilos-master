using ES.Web.Areas.EsAdmin.Services;
using ES.Web.Models;
using ES.Web.Models.Client;
using ES.Web.Services;


namespace ES.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly HomePageService _homePageService;
        private readonly CategoryService _categoryService;
        private readonly PageService _pageService;

        public HomeController(HomePageService homePageService, CategoryService categoryService, PageService pageService)
        {
            _homePageService = homePageService;
            _categoryService = categoryService;
            _pageService = pageService;
        }

        public async Task<IActionResult> Index()
        {
            var pageIds = new List<int> { 3, 38, 39, 40, 41, 42 };

            // Execute sequentially instead of concurrently
            var pages = new List<PageViewModel>();
            foreach (var id in pageIds)
            {
                var page = await _pageService.GetPageByIdAsync(id) ?? new PageViewModel();
                pages.Add(page);
            }

            HomeViewModel model = new()
            {
                MainSlider = await _homePageService.GetMainSliderAsync() ?? new List<PageViewModel>(),
                HomeCounter = await _categoryService.GetCategoryWithPagesByIdAsync(3) ?? new CategoryWithPagesViewModel(),
                HomeWelcome = await _categoryService.GetCategoryWithPagesByIdAsync(5) ?? new CategoryWithPagesViewModel(),
                SliderArticles = await _categoryService.GetCategoryWithPagesByIdAsync(3) ?? new CategoryWithPagesViewModel(),
                HomeProducts = await _categoryService.GetCategoryWithPagesByIdAsync(6,9) ?? new CategoryWithPagesViewModel(),
                HomeServices = await _categoryService.GetCategoryWithPagesByIdAsync(7, 9) ?? new CategoryWithPagesViewModel(),
                Pages = pages,
                NewsTicker = await _categoryService.GetCategoryWithPagesByIdAsync(4, 10) ?? new CategoryWithPagesViewModel(),
                HomeNews = await _categoryService.GetCategoryWithPagesByIdAsync(4, 3) ?? new CategoryWithPagesViewModel(),
                OurBlogs = await _categoryService.GetCategoryWithPagesByIdAsync(4, 3) ?? new CategoryWithPagesViewModel(),
                FooterHomeSlider = await _categoryService.GetCategoryWithPagesByIdAsync(12) ?? new CategoryWithPagesViewModel(),
            };

            return View(model);
        }
    }
}
