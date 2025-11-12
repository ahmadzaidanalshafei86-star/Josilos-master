using ES.Web.Services;

namespace ES.Web.Controllers.client
{
    public class CareersController : Controller
    {
        private readonly CareerService _careerService;

        public CareersController(CareerService careerService)
        {
            _careerService = careerService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _careerService.GetCareersAndFormAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(int careerId, IFormCollection formData)
        {
            var files = HttpContext.Request.Form.Files;

            if (!ModelState.IsValid || careerId <= 0)
            {
                ViewData["CareerId"] = careerId;
                var careersDto = await _careerService.GetCareersAndFormAsync();
                return View("Index", careersDto);
            }

            // Convert IFormCollection to Dictionary<string, string[]> for easier handling in the service
            var formDataDict = formData.ToDictionary(
                k => k.Key,
                v => v.Value.ToArray() // Store all values as an array
            );

            var success = await _careerService.SaveCareerApplicationAsync(careerId, formDataDict, files);
            if (success)
            {
                TempData["SuccessMessage"] = "Your application has been submitted successfully!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "An error occurred while submitting your application.");
            ViewData["CareerId"] = careerId;
            var careersDtoError = await _careerService.GetCareersAndFormAsync();
            return View("Index", careersDtoError);
        }
    }
}
