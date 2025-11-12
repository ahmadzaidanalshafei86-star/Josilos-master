using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;
using ES.Web.Areas.EsAdmin.Services;


namespace ES.Web.Areas.EsAdmin.Controllers
{
    [Area("EsAdmin")]
    public class ProductLabelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILanguageService _languageService;
        private readonly LanguagesRepository _languagesRepository;

        public ProductLabelsController(ApplicationDbContext context,
            LanguagesRepository languagesRepository,
            ILanguageService languageService)
        {
            _context = context;
            _languagesRepository = languagesRepository;
            _languageService = languageService;
        }

        [Authorize(Permissions.Productlabels.Read)]
        public async Task<IActionResult> Index()
        {
            var Labels = await _context.ProductLabels
                .Select(label => new ProductLabelViewModel
                {
                    Id = label.Id,
                    Name = label.Name,
                    CreatedAt = label.CreatedAt
                })
                .ToListAsync();

            return View(Labels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Productlabels.Create)]
        public async Task<IActionResult> Create(ProductLabelViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                ModelState.AddModelError("Name", "Label name is required.");

            if (!ModelState.IsValid)
            {
                var labels = await _context.ProductLabels
                    .Select(label => new ProductLabelViewModel
                    {
                        Id = label.Id,
                        Name = label.Name,
                        CreatedAt = label.CreatedAt
                    })
                    .ToListAsync();
                return View("Index", labels);
            }

            await _context.ProductLabels.AddAsync(new ProductLabel
            {
                Name = model.Name,
                LanguageId = await _languagesRepository.GetLanguageByCode(await _languageService.GetDefaultDbCultureAsync()),
            });
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Productlabels.Update)]
        public async Task<IActionResult> Update(ProductLabelViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                ModelState.AddModelError("Name", "Label name is required.");

            if (!ModelState.IsValid)
            {
                var labels = await _context.ProductLabels
                    .Select(label => new ProductLabelViewModel
                    {
                        Id = label.Id,
                        Name = label.Name,
                        CreatedAt = label.CreatedAt
                    })
                    .ToListAsync();
                return View("Index", labels);
            }

            var label = await _context.ProductLabels.FindAsync(model.Id);
            if (label == null)
                return NotFound();

            label.Name = model.Name;
            _context.ProductLabels.Update(label);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.HasClaim("Permission", Permissions.Productlabels.Delete))
                return StatusCode(403);

            var label = await _context.ProductLabels.FindAsync(id);
            if (label == null)
                return NotFound();

            _context.ProductLabels.Remove(label);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
