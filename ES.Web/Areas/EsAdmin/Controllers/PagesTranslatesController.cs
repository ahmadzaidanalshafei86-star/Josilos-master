using ES.Web.Areas.EsAdmin.Helpers;
using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class PagesTranslatesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PageTranslatesRepository _pageTranslatesRepository;
        private readonly PagesRepository _pagesRepository;
        private readonly RowPermission _rowPermission;

        public PagesTranslatesController(PagesRepository pagesRepository,
            PageTranslatesRepository pageTranslatesRepository,
            RoleManager<IdentityRole> roleManager,
            RowPermission rowPermission)
        {
            _pagesRepository = pagesRepository;
            _pageTranslatesRepository = pageTranslatesRepository;
            _roleManager = roleManager;
            _rowPermission = rowPermission;
        }

        [Authorize(Permissions.Pages.Read)]
        public async Task<IActionResult> Index(int pageId)
        {
            var page = await _pagesRepository.GetPageByIdWithTranslationsAsync(pageId);

            PageTranslatesViewModel model = new()
            {
                PageId = pageId,
                PageTitle = page.Title,
                PageDefaultLang = page.Language!.Code,
                CreatedDate = page.CreatedDate,
                PreEnteredTranslations = page.PageTranslates?.ToList(),
            };
            return View(model);
        }

        [HttpGet]
        [Authorize(Permissions.Pages.Create)]
        public async Task<IActionResult> Create(int pageId)
        {
            var model = await _pageTranslatesRepository.InitializePageTranslatesFormViewModelAsync(pageId);
            model.PageId = pageId;

            var page = await _pagesRepository.GetPageByIdAsync(pageId);

            model.Title = page.Title;
            model.ShortDescription = page.ShortDescription;
            model.LongDescription = page.LongDescription;
            model.MetaDescription = page.MetaDescription;
            model.MetaKeywords = page.MetaKeywords;

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Permissions.Pages.Create)]
        public async Task<IActionResult> Create(PageTranslationFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await _pageTranslatesRepository.InitializePageTranslatesFormViewModelAsync(model.PageId);
                return View("Form", model);
            }

            PageTranslate pageTranslate = new()
            {
                Title = model.Title,
                ShortDescription = model.ShortDescription,
                LongDescription = model.LongDescription,
                MetaDescription = model.MetaDescription,
                MetaKeywords = model.MetaKeywords,
                CreatedDate = DateTime.UtcNow,
                LanguageId = (int)model.LanguageId,
                PageId = model.PageId,
            };

            await _pageTranslatesRepository.AddPageTranslateAsync(pageTranslate);

            return RedirectToAction("Index", new { pageId = model.PageId });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int pageId, int Translateid)
        {
            var translate = await _pageTranslatesRepository.GetPageTranslateByIdAsync(Translateid);

            //Row level Permission Check
            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();
            var RowLevelPermission = await _rowPermission
                                          .HasRowLevelPermissionAsync(role.Id,
                                           TablesNames.PagesOfCategory,
                                           translate.Page.CategoryId,
                                           CrudOperations.Update);

            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return Redirect("/Identity/Account/AccessDenied");

            PageTranslationFormViewModel model = new()
            {
                TranslationId = Translateid,
                Title = translate.Title,
                ShortDescription = translate.ShortDescription,
                LongDescription = translate.LongDescription,
                MetaDescription = translate.MetaDescription,
                MetaKeywords = translate.MetaKeywords,
            };

            model = await _pageTranslatesRepository.InitializePageTranslatesFormViewModelAsync(pageId, model);
            model.PageId = pageId;

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(PageTranslationFormViewModel model, int pageId)
        {
            if (!ModelState.IsValid)
            {
                model = await _pageTranslatesRepository.InitializePageTranslatesFormViewModelAsync(pageId, model);
                model.PageId = pageId;
                return View("Form", model);
            }

            var translate = await _pageTranslatesRepository.GetPageTranslateByIdAsync(model.TranslationId);

            //Row level Permission Check
            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();
            var RowLevelPermission = await _rowPermission
                                          .HasRowLevelPermissionAsync(role.Id,
                                           TablesNames.PagesOfCategory,
                                           translate.Page.CategoryId,
                                           CrudOperations.Update);

            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return Redirect("/Identity/Account/AccessDenied");

            translate.Title = model.Title;
            translate.ShortDescription = model.ShortDescription;
            translate.LongDescription = model.LongDescription;
            translate.MetaDescription = model.MetaDescription;
            translate.MetaKeywords = model.MetaKeywords;


            _pageTranslatesRepository.UpdatepageTranslate(translate);

            return RedirectToAction("Index", new { pageId = model.PageId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var translate = await _pageTranslatesRepository.GetPageTranslateByIdAsync(id);

            if (translate == null)
                return StatusCode(404);
            //Row level Permission Check
            var role = await _roleManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Role));
            if (role is null)
                return NotFound();
            var RowLevelPermission = await _rowPermission
                                          .HasRowLevelPermissionAsync(role.Id,
                                           TablesNames.PagesOfCategory,
                                           translate.Page.CategoryId,
                                           CrudOperations.Delete);

            if (!RowLevelPermission && !(role.Name == AppRoles.SuperAdmin))
                return StatusCode(403);


            var result = await _pageTranslatesRepository.DeleteTranslationAsync(id); //returns true if deleted successfully
            if (result)
                return StatusCode(200);

            return BadRequest();

        }
    }
}
