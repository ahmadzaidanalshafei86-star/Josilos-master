using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;
using ES.Web.Areas.EsAdmin.Services;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class CareersController : Controller
    {
        private readonly CareersRepository _careersRepository;
        private readonly ILanguageService _languageService;
        private readonly LanguagesRepository _languagesRepository;

        public CareersController(CareersRepository careersRepository,
            ILanguageService languageService,
            LanguagesRepository languagesRepository,
            ApplicationDbContext context)
        {
            _careersRepository = careersRepository;
            _languageService = languageService;
            _languagesRepository = languagesRepository;
        }

        [HttpGet]
        [Authorize(Permissions.Careers.Read)]
        public async Task<IActionResult> Index()
        {
            var careers = await _careersRepository.GetCareersAsync();

            return View(careers);
        }

        [HttpGet]
        [Authorize(Permissions.Careers.Create)]
        public IActionResult Create()
        {
            CareerFormViewModel model = new();
            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Careers.Create)]
        public async Task<IActionResult> Create(CareerFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            var career = new Career
            {
                JobTitle = model.JobTitle,
                RefNumber = model.RefNumber,
                Description = model.Description,
                Location = model.Location,
                EmploymentType = model.EmploymentType,
                Salary = model.Salary,
                EnviromentType = model.EnviromentType,
                IsActive = model.IsActive,
                LanguageId = await _languagesRepository.GetLanguageByCode(await _languageService.GetDefaultDbCultureAsync()),
            };
            await _careersRepository.AddCareerAsync(career);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Permissions.Careers.Update)]
        public async Task<IActionResult> Edit(int CareerId)
        {
            var career = await _careersRepository.GetCareerByIdAsync(CareerId);
            if (career == null)
                return NotFound();
            CareerFormViewModel model = new()
            {
                Id = career.Id,
                JobTitle = career.JobTitle,
                RefNumber = career.RefNumber,
                Description = career.Description,
                Location = career.Location,
                EmploymentType = career.EmploymentType,
                EnviromentType = career.EnviromentType,
                Salary = career.Salary,
                IsActive = career.IsActive,
            };
            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Careers.Update)]
        public async Task<IActionResult> Edit(CareerFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            var career = await _careersRepository.GetCareerByIdAsync(model.Id);
            if (career == null)
                return NotFound();

            career.JobTitle = model.JobTitle;
            career.RefNumber = model.RefNumber;
            career.Description = model.Description;
            career.Location = model.Location;
            career.EmploymentType = model.EmploymentType;
            career.EnviromentType = model.EnviromentType;
            career.Salary = model.Salary;
            career.IsActive = model.IsActive;

            await _careersRepository.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        [Authorize(Permissions.Careers.Read)]
        public async Task<IActionResult> ViewApplications(int careerId, int page = 1, int pageSize = 6, string search = "",
            string sort = "", bool showArchived = false, bool showUnreviewedOnly = false)
        {
            var (applications, totalCount) = await _careersRepository.GetApplicationsByCareerIdAsync(careerId, page, pageSize, search, sort,
                showArchived, showUnreviewedOnly);
            ViewBag.TotalCount = totalCount;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.CareerId = careerId;
            ViewBag.Search = search;
            ViewBag.Sort = sort;
            ViewBag.ShowArchived = showArchived;
            ViewBag.ShowUnreviewedOnly = showUnreviewedOnly;
            return View(applications);
        }

        [HttpPost]
        [Authorize(Permissions.Careers.Update)]
        public async Task<IActionResult> BulkAction(int careerId, int[] selectedIds, string action)
        {
            if (selectedIds?.Length > 0)
            {
                switch (action)
                {
                    case "toggle-reviewed":
                        await _careersRepository.MarkAsReviewedAsync(selectedIds);
                        break;
                    case "toggle-archive":
                        await _careersRepository.ArchiveApplicationsAsync(selectedIds);
                        break;
                }
            }
            return RedirectToAction("ViewApplications", new { careerId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.HasClaim("Permission", Permissions.Careers.Delete))
                return StatusCode(403);
            var career = await _careersRepository.GetCareerByIdAsync(id);

            if (career is null)
                return NotFound();

            await _careersRepository.DeleteCareerAsync(career);

            return StatusCode(200);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            if (!User.HasClaim("Permission", Permissions.Careers.Update))
                return StatusCode(403);
            var career = await _careersRepository.GetCareerByIdAsync(id);

            if (career is null)
                return NotFound();

            career.IsActive = !career.IsActive;

            await _careersRepository.SaveChangesAsync();

            return StatusCode(200);
        }
    }
}
