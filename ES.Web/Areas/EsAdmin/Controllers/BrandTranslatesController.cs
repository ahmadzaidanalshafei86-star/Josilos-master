using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class BrandTranslatesController : Controller
    {
        private readonly BrandsRepository _brandsRepository;
        private readonly BrandTranslatesRepository _brandTranslatesRepository;

        public BrandTranslatesController(BrandsRepository brandsRepository,
            BrandTranslatesRepository brandTranslatesRepository)
        {
            _brandsRepository = brandsRepository;
            _brandTranslatesRepository = brandTranslatesRepository;
        }

        [HttpGet]
        [Authorize(Permissions.Brands.Read)]
        public async Task<IActionResult> Index(int brandId)
        {
            var brand = await _brandsRepository.GetBrandWithTranslationAsync(brandId);

            if (brand == null)
                return NotFound();

            BrandTranslateViewModel model = new()
            {
                BrandId = brandId,
                BrandName = brand.Name,
                BrandDefaultLang = brand.Language.Code,
                CreatedDate = brand.CreatedAt,
                PreEnteredTranslations = brand.BrandTranslates?.ToList(),
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetTranslationForm(int brandId, int? translationId = null)
        {
            BrandTranslateFormViewModel model;
            // in case of edit operation
            if (translationId.HasValue)
            {
                if (!User.HasClaim("Permission", Permissions.Brands.Update))
                    return StatusCode(403);

                var translation = await _brandTranslatesRepository.GetBrandTranslateByIdAsync(translationId.Value);

                if (translation == null)
                    return NotFound();

                model = new BrandTranslateFormViewModel
                {
                    TranslationId = translation.Id,
                    BrandId = translation.BrandId,
                    Name = translation.Name,
                    LanguageId = translation.LanguageId,
                };

                model = await _brandTranslatesRepository.InitializeBrandTranslatesFormViewModelAsync(brandId, model);
            }
            // Initialize for create operation
            else
            {
                if (!User.HasClaim("Permission", Permissions.Brands.Create))
                    return StatusCode(403);
                model = await _brandTranslatesRepository.InitializeBrandTranslatesFormViewModelAsync(brandId);
                model.BrandId = brandId;
            }

            return PartialView("_Form", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveTranslation(BrandTranslateFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.TranslationId == 0) // Create new translation
                {
                    BrandTranslate brandTranslate = new()
                    {
                        BrandId = model.BrandId,
                        Name = model.Name,
                        LanguageId = model.LanguageId,
                    };
                    await _brandTranslatesRepository.AddBrandTranslateAsync(brandTranslate);
                }
                else // Update existing translation
                {
                    var translation = await _brandTranslatesRepository.GetBrandTranslateByIdAsync(model.TranslationId);
                    if (translation is null)
                        return NotFound();

                    translation.Name = model.Name;

                    await _brandTranslatesRepository.UpdateTranslate(translation);
                }

                return Json(new { success = true });
            }

            return PartialView("_TranslationForm", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.HasClaim("Permission", Permissions.Brands.Delete))
                return StatusCode(403);

            var translate = await _brandTranslatesRepository.GetBrandTranslateByIdAsync(id);

            if (translate == null)
                return StatusCode(404);

            await _brandTranslatesRepository.DeleteTranslationAsync(id); //returns true if deleted successfully

            return StatusCode(200);
        }
    }
}
