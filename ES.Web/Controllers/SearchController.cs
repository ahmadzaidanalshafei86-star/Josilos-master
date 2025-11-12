using ES.Web.Services;

namespace ES.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly SearchService _searchService;

        public SearchController(SearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<IActionResult> Index(string query, string sortOrder = "desc")
        {
            var results = await _searchService.SearchAsync(query, sortOrder);
            ViewData["Query"] = query;
            ViewData["SortOrder"] = sortOrder;
            return View(results);
        }
    }
}
