using ES.Web.Areas.EsAdmin.Helpers;
using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class CategoriesTranslatesController : Controller
    {
        private readonly CategoriesRepository _categoriesRepository;
        private readonly CategoriesTranslatesRepository _categoriesTranslatesRepository;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly RowPermission _rowPermission;
        public CategoriesTranslatesController(CategoriesRepository categoriesRepository,
            CategoriesTranslatesRepository categoriesTranslatesRepository,
            RowPermission rowPermission,
            RoleManager<IdentityRole> roleManager)
        {
            _categoriesRepository = categoriesRepository;
            _categoriesTranslatesRepository = categoriesTranslatesRepository;
            _rowPermission = rowPermission;
            _roleManager = roleManager;
        }

        [Authorize(Permissions.Categories.Read)]
        public async Task<IActionResult> Index(int categoryId)
        {
            var category = await _categoriesRepository.GetCategoryByIdWithTranslationsAsync(categoryId);

            CategoryTranslatesViewModel model = new()
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
        [Authorize(Permissions.Categories.Create)]
        public async Task<IActionResult> Create(int categoryId)
        {
            var model = await _categoriesTranslatesRepository.InitializeCategoryTranslatesFormViewModelAsync(categoryId);
            model.CategoryId = categoryId;

            var category = await _categoriesRepository.GetCategoryByIdAsync(categoryId);

            model.Name = category.Name;
            model.ShortDescription = category.ShortDescription;
            model.LongDescription = category.LongDescription;
            model.MetaDescription = category.MetaDescription;
            model.MetaKeywords = category.MetaKeywords;

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Categories.Create)]
        public async Task<IActionResult> Create(CategoryTranslationFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _categoriesTranslatesRepository.InitializeCategoryTranslatesFormViewModelAsync(model.CategoryId);
                return View("Form", model);
            }

            CategoryTranslate categoryTranslate = new()
            {
                Name = model.Name,
                ShortDescription = model.ShortDescription,
                LongDescription = model.LongDescription,
                MetaDescription = model.MetaDescription,
                MetaKeywords = model.MetaKeywords,
                CreatedDate = DateTime.UtcNow,
                LanguageId = (int)model.LanguageId,
                CategoryId = model.CategoryId,
            };

            await _categoriesTranslatesRepository.AddCategoryTranslateAsync(categoryTranslate);

            return RedirectToAction("Index", new { categoryId = model.CategoryId });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int categoryId, int Translateid)
        {
            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();

            var RowLevelPermission = await _rowPermission
                                            .HasRowLevelPermissionAsync(role.Id,
                                            TablesNames.Categories,
                                            categoryId,
                                            CrudOperations.Update);

            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return Redirect("/Identity/Account/AccessDenied");

            var translate = await _categoriesTranslatesRepository.GetCategoryTranslateByIdAsync(Translateid);

            CategoryTranslationFormViewModel model = new()
            {
                TranslationId = Translateid,
                Name = translate.Name,
                ShortDescription = translate.ShortDescription,
                LongDescription = translate.LongDescription,
                MetaDescription = translate.MetaDescription,
                MetaKeywords = translate.MetaKeywords,
            };

            model = await _categoriesTranslatesRepository.InitializeCategoryTranslatesFormViewModelAsync(categoryId, model);
            model.CategoryId = categoryId;

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(CategoryTranslationFormViewModel model, int categoryId)
        {

            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();

            var RowLevelPermission = await _rowPermission.HasRowLevelPermissionAsync(role.Id, TablesNames.Categories, categoryId, CrudOperations.Update);

            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return Redirect("/Identity/Account/AccessDenied");

            if (!ModelState.IsValid)
            {
                model = await _categoriesTranslatesRepository.InitializeCategoryTranslatesFormViewModelAsync(categoryId, model);
                model.CategoryId = categoryId;
                return View("Form", model);
            }

            var translate = await _categoriesTranslatesRepository.GetCategoryTranslateByIdAsync(model.TranslationId);

            translate.Name = model.Name;
            translate.ShortDescription = model.ShortDescription;
            translate.LongDescription = model.LongDescription;
            translate.MetaDescription = model.MetaDescription;
            translate.MetaKeywords = model.MetaKeywords;


            _categoriesTranslatesRepository.UpdateCategory(translate);

            return RedirectToAction("Index", new { categoryId = model.CategoryId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int translationId, int categoryId)
        {
            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();

            var RowLevelPermission = await _rowPermission.HasRowLevelPermissionAsync(role.Id, TablesNames.Categories, categoryId, CrudOperations.Delete);

            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return StatusCode(403);

            var result = await _categoriesTranslatesRepository.DeleteTranslationAsync(translationId); //returns true if deleted successfully
            if (result)
                return StatusCode(200);

            return BadRequest();

        }
    }
}
