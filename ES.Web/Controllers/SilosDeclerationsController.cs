


using ES.Web.Services;

namespace ES.Web.Controllers.client
{
    public class SilosDeclerationsController : Controller
    {
        private readonly SilosDeclerationsService _silosDeclerationsService;

        public SilosDeclerationsController(SilosDeclerationsService silosDeclerationsService)
        {
            _silosDeclerationsService = silosDeclerationsService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _silosDeclerationsService.GetAllAsync();

            return View(model);
        }

        
    }
}

