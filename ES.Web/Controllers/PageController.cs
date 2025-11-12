using Microsoft.Extensions.Localization;
using ES.Web.Services;

namespace ES.Web.Controllers
{
    [AllowAnonymous]
    public class PageController : Controller
    {
        private readonly PageService _pageService;
        private readonly IStringLocalizer<PageController> _localizer;

        public PageController(PageService pageService, IStringLocalizer<PageController> localizer)
        {
            _pageService = pageService;
            _localizer = localizer;
        }

        [Route("Page/{slug}")]
        [HttpGet]
        public async Task<IActionResult> Index(string slug)
        {
            var pageModel = await _pageService.GetPageBySlugAsync(slug);

            return View("pageDetails", pageModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(int formId, string PageSlug, IFormCollection formData)
        {
            var files = HttpContext.Request.Form.Files;

            var formDataDict = formData.ToDictionary(
                k => k.Key,
                v => v.Value.ToArray()
            );

            await _pageService.SavePageFormAsync(formId, formDataDict, files);

            TempData["SuccessMessage"] = _localizer["Your message has been submitted successfully!"].Value;
            return RedirectToAction("Index", new { slug = PageSlug });
        }

    }
}
