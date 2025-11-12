using ES.Web.Services;

namespace ES.Web.ViewComponents
{
    public class TopViewComponent : ViewComponent
    {
        private readonly TopService _TopService;

        public TopViewComponent(TopService TopService)
        {
            _TopService = TopService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var Top = await _TopService.GetTopAsync();

            if (Top is null)
                return Content("No Top found");

            return View(Top);
        }
    }
}
