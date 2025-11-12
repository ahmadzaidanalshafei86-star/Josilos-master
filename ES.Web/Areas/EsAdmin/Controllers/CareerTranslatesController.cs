using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class CareerTranslatesController : Controller
    {
        private readonly CareersRepository _careersRepository;
        private readonly CareerTranslatesRepository _careersTranslatesRepository;

        public CareerTranslatesController(CareersRepository careersRepository,
            CareerTranslatesRepository careersTranslatesRepository)
        {
            _careersRepository = careersRepository;
            _careersTranslatesRepository = careersTranslatesRepository;
        }

        [HttpGet]
        [Authorize(Permissions.Careers.Read)]
        public async Task<IActionResult> Index(int careerId)
        {
            var career = await _careersRepository.GetCareerWithTranslationAsync(careerId);

            if (career == null)
                return NotFound();

            CareerTranslateViewModel model = new()
            {
                CareerId = careerId,
                CareerName = career.JobTitle,
                CareerDefaultLang = career.Language.Code,
                CreatedDate = career.CreatedAt,
                PreEnteredTranslations = career.CareerTranslates?.ToList()
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Permissions.Careers.Create)]
        public async Task<IActionResult> Create(int careerId)
        {
            var model = await _careersTranslatesRepository.InitializeCareerTranslatesFormViewModelAsync(careerId);
            model.CareerId = careerId;

            var career = await _careersRepository.GetCareerByIdAsync(careerId);

            if (career == null)
                return NotFound();

            model.JobTitle = career.JobTitle;
            model.Description = career.Description;
            model.Location = career.Location;

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Careers.Create)]
        public async Task<IActionResult> Create(CareerTranslatesFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);
            var careerTranslate = new CareerTranslate
            {
                CareerId = model.CareerId,
                LanguageId = (int)model.LanguageId!,
                JobTitle = model.JobTitle,
                Description = model.Description,
                Location = model.Location
            };
            await _careersTranslatesRepository.AddCareerTranslateAsync(careerTranslate);

            return RedirectToAction("Index", new { careerId = model.CareerId });
        }

        [HttpGet]
        [Authorize(Permissions.Careers.Update)]
        public async Task<IActionResult> Edit(int careerId, int Translateid)
        {
            var careerTranslate = await _careersTranslatesRepository.GetCareerTranslateByIdAsync(Translateid);

            if (careerTranslate == null)
                return NotFound();

            var model = new CareerTranslatesFormViewModel
            {
                TranslationId = careerTranslate.Id,
                JobTitle = careerTranslate.JobTitle,
                Description = careerTranslate.Description,
                Location = careerTranslate.Location
            };

            model = await _careersTranslatesRepository.InitializeCareerTranslatesFormViewModelAsync(careerId, model);
            model.CareerId = careerId;

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Careers.Update)]
        public async Task<IActionResult> Edit(CareerTranslatesFormViewModel model, int careerId)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            var careerTranslate = await _careersTranslatesRepository.GetCareerTranslateByIdAsync(model.TranslationId);
            if (careerTranslate == null)
                return NotFound();

            careerTranslate.JobTitle = model.JobTitle;
            careerTranslate.Description = model.Description;
            careerTranslate.Location = model.Location;
            await _careersTranslatesRepository.SaveChangesAsync();

            return RedirectToAction("Index", new { careerId = model.CareerId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int translationId, int careerId)
        {
            if (!User.HasClaim("Permission", Permissions.Careers.Delete))
                return StatusCode(403);

            var succes = await _careersTranslatesRepository.DeleteTranslationAsync(translationId);

            if (!succes)
                return NotFound();


            return RedirectToAction("Index", new { CareerId = careerId });
        }
    }
}
