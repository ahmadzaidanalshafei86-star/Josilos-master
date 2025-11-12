using ES.Web.Services;

namespace ES.Web.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly FooterService _footerService;

        public FooterViewComponent(FooterService footerService)
        {
            _footerService = footerService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var footer = await _footerService.GetFooterAsync();

            if (footer is null)
                return Content("No footer found");

            return View(footer);
        }
    }
}
