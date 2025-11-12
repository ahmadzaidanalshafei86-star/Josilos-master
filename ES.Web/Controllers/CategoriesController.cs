using ES.Web.Services;

namespace ES.Web.Controllers.Categories
{
    [AllowAnonymous]
    public class CategoriesController : Controller
    {
        private readonly CategoryService _categoryService;
        public CategoriesController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [Route("Categories/{slug}")]
        public async Task<IActionResult> Index(string slug)
        {
            var category = await _categoryService.GetCategoryWithPagesBySlugAsync(slug);

            if (category == null)
                return NotFound();

            return View("ViewCategory", category);
        }
    }
}
