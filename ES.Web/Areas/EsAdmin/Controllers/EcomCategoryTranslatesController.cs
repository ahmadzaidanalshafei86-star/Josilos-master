using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class EcomCategoryTranslatesController : Controller
    {
        private readonly EcomCategoriesRepository _ecomCategoriesRepository;
        private readonly EcomCategoryTranslatesRepository _ecomCategoryTranslatesRepository;

        public EcomCategoryTranslatesController(EcomCategoriesRepository ecomCategoriesRepository,
            EcomCategoryTranslatesRepository ecomCategoryTranslatesRepository)
        {
            _ecomCategoriesRepository = ecomCategoriesRepository;
            _ecomCategoryTranslatesRepository = ecomCategoryTranslatesRepository;
        }

        [HttpGet]
        [Authorize(Permissions.ProductCategories.Read)]
        public async Task<IActionResult> Index(int categoryId)
        {
            var category = await _ecomCategoriesRepository.GetCategoryWithTranslationAsync(categoryId);

            if (category is null)
                return NotFound();

            EcomCategoryTranslatesViewModel model = new()
            {
                CategoryId = categoryId,
                CategoryName = category.Name,
                CategoryDefaultLang = category.Language.Code,
                CreatedDate = category.CreatedDate,
                PreEnteredTranslations = category.CategoryTranslates?.ToList(),
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Permissions.ProductCategories.Create)]
        public async Task<IActionResult> Create(int categoryId)
        {
            var model = await _ecomCategoryTranslatesRepository.InitializeCategoryTranslatesFormViewModelAsync(categoryId);
            model.CategoryId = categoryId;

            var category = await _ecomCategoriesRepository.GetCategoryByIdAsync(categoryId);

            model.Name = category.Name;
            model.ShortDescription = category.ShortDescription;
            model.LongDescription = category.LongDescription;
            model.MetaDescription = category.MetaDescription;
            model.MetaKeywords = category.MetaKeywords;

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.ProductCategories.Create)]
        public async Task<IActionResult> Create(EcomCategoryTranslatesFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            var categoryTranslate = new EcomCategoryTranslate
            {
                LanguageId = (int)model.LanguageId!,
                Name = model.Name,
                EcomCategoryId = model.CategoryId,
                ShortDescription = model.ShortDescription,
                LongDescription = model.LongDescription,
                MetaDescription = model.MetaDescription,
                MetaKeywords = model.MetaKeywords
            };
            await _ecomCategoryTranslatesRepository.AddCategoryTranslateAsync(categoryTranslate);
            return RedirectToAction("Index", new { categoryId = model.CategoryId });
        }

        [HttpGet]
        [Authorize(Permissions.ProductCategories.Update)]
        public async Task<IActionResult> Edit(int categoryId, int Translateid)
        {
            var categoryTranslate = await _ecomCategoryTranslatesRepository.GetCategoryTranslateByIdAsync(Translateid);

            if (categoryTranslate is null)
                return NotFound();

            var model = new EcomCategoryTranslatesFormViewModel
            {
                TranslationId = categoryTranslate.Id,
                Name = categoryTranslate.Name,
                ShortDescription = categoryTranslate.ShortDescription,
                LongDescription = categoryTranslate.LongDescription,
                MetaDescription = categoryTranslate.MetaDescription,
                MetaKeywords = categoryTranslate.MetaKeywords
            };

            model = await _ecomCategoryTranslatesRepository.InitializeCategoryTranslatesFormViewModelAsync(categoryId, model);
            model.CategoryId = categoryId;
            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.ProductCategories.Update)]
        public async Task<IActionResult> Edit(EcomCategoryTranslatesFormViewModel model, int categoryId)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            var translate = await _ecomCategoryTranslatesRepository.GetCategoryTranslateByIdAsync(model.TranslationId);

            if (translate is null) return NotFound();

            translate.Name = model.Name;
            translate.ShortDescription = model.ShortDescription;
            translate.LongDescription = model.LongDescription;
            translate.MetaDescription = model.MetaDescription;
            translate.MetaKeywords = model.MetaKeywords;

            await _ecomCategoryTranslatesRepository.UpdateCategoryTrarnslation(translate);

            return RedirectToAction("Index", new { categoryId = model.CategoryId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int translationId, int categoryId)
        {
            if (!User.HasClaim("Permission", Permissions.ProductCategories.Delete))
                return StatusCode(403);

            await _ecomCategoryTranslatesRepository.DeleteTranslationAsync(translationId);

            return RedirectToAction("Index", new { categoryId });

        }
    }
}