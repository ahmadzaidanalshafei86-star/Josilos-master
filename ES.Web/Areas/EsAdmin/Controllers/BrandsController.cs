using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;
using ES.Web.Areas.EsAdmin.Services;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class BrandsController : Controller
    {
        private readonly BrandsRepository _brandsRepository;
        private readonly IImageService _imageService;
        private readonly SlugService _slugService;
        private readonly ILanguageService _languageService;
        private readonly LanguagesRepository _languagesRepository;

        public BrandsController(BrandsRepository brandsRepository,
            IImageService imageService,
            SlugService slugService,
            ILanguageService languageService,
            LanguagesRepository languagesRepository)
        {
            _brandsRepository = brandsRepository;
            _imageService = imageService;
            _slugService = slugService;
            _languageService = languageService;
            _languagesRepository = languagesRepository;
        }

        [HttpGet]
        [Authorize(Permissions.Brands.Read)]
        public async Task<IActionResult> Index()
        {
            var brands = await _brandsRepository.GetBrandsAsync();
            return View(brands);
        }

        [HttpGet]
        [Authorize(Permissions.Brands.Create)]
        public IActionResult Create()
        {
            BrandFormViewModel model = new();
            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Brands.Create)]
        public async Task<IActionResult> Create(BrandFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            Brand brand = new()
            {
                Name = model.Name,
                IsActive = model.IsActive,
                Slug = _slugService.GenerateUniqueSlug(model.Name, nameof(Brand)),
                LanguageId = await _languagesRepository.GetLanguageByCode(await _languageService.GetDefaultDbCultureAsync()),
            };

            var brandId = await _brandsRepository.AddBrandAsync(brand);

            var LogoImageName = $"{brandId}_logoImage{Path.GetExtension(model.Logo.FileName)}";

            var (isUploaded, errorMessage) = await _imageService.UploadASync(model.Logo, LogoImageName, "/images/Brands");
            if (isUploaded)
            {
                brand.LogoUrl = LogoImageName;
                brand.LogoAltName = model.Logo.FileName;

                await _brandsRepository.UpdateBrandAsync(brand);
            }
            else
            {
                ModelState.AddModelError(nameof(model.Logo), errorMessage!);
                return View("Form", model);
            }

            return View("Form", model);
        }

        [HttpGet]
        [Authorize(Permissions.Brands.Update)]
        public async Task<IActionResult> Edit(int brandId)
        {
            var brand = await _brandsRepository.GetBrandByIdAsync(brandId);
            if (brand is null)
                return NotFound();
            BrandFormViewModel model = new()
            {
                Id = brand.Id,
                Name = brand.Name,
                IsActive = brand.IsActive,
                LogoUrl = brand.LogoUrl,
                LogoAltName = brand.LogoAltName
            };
            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Brands.Update)]
        public async Task<IActionResult> Edit(BrandFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            var brand = await _brandsRepository.GetBrandByIdAsync(model.Id);
            if (brand is null)
                return NotFound();

            brand.Name = model.Name;
            brand.IsActive = model.IsActive;

            // Handle Logo
            if (model.Logo != null)
            {
                if (!string.IsNullOrEmpty(brand.LogoUrl))
                    _imageService.Delete($"/images/Brands/{brand.LogoUrl}");

                var LogoImageName = $"{brand.Id}_logoImage{Path.GetExtension(model.Logo.FileName)}";
                var (isUploaded, errorMessage) = await _imageService.UploadASync(model.Logo, LogoImageName, "/images/Brands");

                if (isUploaded)
                {
                    brand.LogoUrl = LogoImageName;
                    brand.LogoAltName = model.Logo.FileName;
                }
                else
                {
                    ModelState.AddModelError(nameof(model.Logo), errorMessage!);
                    return View("Form", model);
                }
            }

            await _brandsRepository.UpdateBrandAsync(brand);

            return View("Form", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.HasClaim("Permission", Permissions.Brands.Delete))
                return StatusCode(403);
            var brand = await _brandsRepository.GetBrandByIdAsync(id);

            if (brand is null)
                return NotFound();

            if (!string.IsNullOrEmpty(brand.LogoUrl))
                _imageService.Delete($"/images/Brands/{brand.LogoUrl}");

            await _brandsRepository.DeleteBrandAsync(brand);

            return StatusCode(200);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            if (!User.HasClaim("Permission", Permissions.Brands.Update))
                return StatusCode(403);
            var brand = await _brandsRepository.GetBrandByIdAsync(id);

            if (brand is null)
                return StatusCode(402);

            brand.IsActive = !brand.IsActive;
            await _brandsRepository.UpdateBrandAsync(brand);

            return StatusCode(200);
        }

        public async Task<IActionResult> AllowBrandName(BrandFormViewModel model)
        {
            var brand = await _brandsRepository.IsBrandNameExist(model.Name);
            var isAllowed = brand is null || brand.Id.Equals(model.Id);

            return Json(isAllowed);
        }
    }
}
