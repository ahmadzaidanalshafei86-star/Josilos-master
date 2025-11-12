using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class MenuItemTranslatesController : Controller
    {
        private readonly MenuItemTranslatesRepository _menuItemTranslatesRepository;
        private readonly MenuItemsRepository _menuItemsRepository;

        public MenuItemTranslatesController(
            MenuItemsRepository menuItemsRepository,
            MenuItemTranslatesRepository menuItemTranslatesRepository)
        {
            _menuItemsRepository = menuItemsRepository;
            _menuItemTranslatesRepository = menuItemTranslatesRepository;
        }
        [HttpGet]
        [Authorize(Permissions.MenuManagment.Read)]
        public async Task<IActionResult> Index(int menuItemId)
        {
            var menuItem = await _menuItemsRepository.GetMenuItemWithTranslationsAsync(menuItemId);

            MenuItemTranslatesViewModel model = new()
            {
                MenuItemId = menuItemId,
                MenuItemTitle = menuItem.Title,
                MenuItemDefaultLang = menuItem.Language!.Code,
                CreatedDate = menuItem.CreatedDate,
                PreEnteredTranslations = menuItem.Translations?.ToList(),
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetTranslationForm(int menuItemId, int? translationId = null)
        {
            MenuItemTranslationFormViewModel model;
            // in case of edit operation
            if (translationId.HasValue)
            {
                if (!User.HasClaim("Permission", Permissions.MenuManagment.Update))
                    return StatusCode(403);

                var translation = await _menuItemTranslatesRepository.GetMenuItemTranslateByIdAsync(translationId.Value);

                if (translation == null)
                    return NotFound();

                model = new MenuItemTranslationFormViewModel
                {
                    TranslationId = translation.Id,
                    MenuItemId = translation.MenuItemId,
                    Title = translation.Title,
                    CustomUrl = translation.CustomUrl,
                    LanguageId = translation.LanguageId,
                };

                model = await _menuItemTranslatesRepository.InitializeMenuItemTranslatesFormViewModelAsync(menuItemId, model);
            }
            // Initialize for create operation
            else
            {
                if (!User.HasClaim("Permission", Permissions.MenuManagment.Create))
                    return StatusCode(403);
                model = await _menuItemTranslatesRepository.InitializeMenuItemTranslatesFormViewModelAsync(menuItemId);
                model.MenuItemId = menuItemId;
            }

            return PartialView("_TranslationForm", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveTranslation(MenuItemTranslationFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.TranslationId == 0) // Create new translation
                {
                    MenuItemTranslate menuItemTranslate = new()
                    {
                        MenuItemId = model.MenuItemId,
                        Title = model.Title,
                        CustomUrl = model.CustomUrl,
                        LanguageId = model.LanguageId,
                        CreatedDate = DateTime.UtcNow,
                    };
                    await _menuItemTranslatesRepository.AddMenuItemTranslateAsync(menuItemTranslate);
                }
                else // Update existing translation
                {
                    var translation = await _menuItemTranslatesRepository.GetMenuItemTranslateByIdAsync(model.TranslationId);
                    if (translation is null)
                        return NotFound();

                    translation.Title = model.Title;
                    translation.CustomUrl = model.CustomUrl;

                    _menuItemTranslatesRepository.UpdateTranslate(translation);
                }

                return Json(new { success = true });
            }

            return PartialView("_TranslationForm", model); // Return form with validation errors
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.HasClaim("Permission", Permissions.MenuManagment.Delete))
                return StatusCode(403);

            var translate = await _menuItemTranslatesRepository.GetMenuItemTranslateByIdAsync(id);

            if (translate == null)
                return StatusCode(404);

            var result = await _menuItemTranslatesRepository.DeleteTranslationAsync(id); //returns true if deleted successfully
            if (result)
                return StatusCode(200);

            return BadRequest();

        }
    }
}
