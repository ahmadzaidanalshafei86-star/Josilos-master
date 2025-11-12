using ES.Web.Areas.EsAdmin.Models;
using ES.Web.Areas.EsAdmin.Repositories;
using ES.Web.Areas.EsAdmin.Services;

namespace ES.Web.Controllers
{
    [Area("EsAdmin")]
    public class MenuManagerController : Controller
    {
        private readonly MenuItemsRepository _menuItemsRepository;
        private readonly CategoriesRepository _categoriesRepository;
        private readonly PagesRepository _pagesRepository;
        private readonly EcomCategoriesRepository _ecomCategoriesRepository;
        private readonly ILanguageService _languageService;
        private readonly LanguagesRepository _languagesRepository;

        public MenuManagerController(PagesRepository pagesRepository,
            CategoriesRepository categoriesRepository,
            MenuItemsRepository menuItemsRepository,
            ILanguageService languageService,
            LanguagesRepository languagesRepository,
            EcomCategoriesRepository ecomCategoriesRepository)
        {
            _pagesRepository = pagesRepository;
            _categoriesRepository = categoriesRepository;
            _menuItemsRepository = menuItemsRepository;
            _languageService = languageService;
            _languagesRepository = languagesRepository;
            _ecomCategoriesRepository = ecomCategoriesRepository;
        }


        [HttpGet]
        [Authorize(Permissions.MenuManagment.Read)]
        public async Task<IActionResult> Index()
        {
            var allmenuItems = await _menuItemsRepository.GetAllMenuItemsAsync();

            MenuItemViewModel model = new()
            {
                Categories = await _categoriesRepository.GetCategoriesSlugsNamesAsync(),
                Pages = await _pagesRepository.GetPagesSlugesAsync(),
                ProductCategories = await _ecomCategoriesRepository.GetCategoriesSlugesAsync(),
                MenuItems = BuildMenuTree(allmenuItems)
            };
            return View("MenuManager", model);
        }

        [HttpPost]
        public async Task<IActionResult> AddMenuItem(MenuItemViewModel model)
        {
            if (!User.HasClaim("Permission", Permissions.MenuManagment.Create))
                return StatusCode(403);

            if (ModelState.IsValid)
            {
                // Simulate database insertion
                var newMenuItem = new MenuItem
                {
                    Title = model.Title,
                    Target = model.Target!,
                    Icon = model.Icon,
                    Type = model.Type,
                    Url = model.URL,
                    Order = 0,// Default order
                    IsPublished = model.IsPublished,
                    LanguageId = await _languagesRepository.GetLanguageByCode(await _languageService.GetDefaultDbCultureAsync()),
                };

                await _menuItemsRepository.AddMenuItemAsync(newMenuItem);

                // Return new menu item details for the frontend
                return Json(new
                {
                    id = newMenuItem.Id,
                    title = newMenuItem.Title,
                    type = newMenuItem.Type,
                    isPublished = newMenuItem.IsPublished,
                });
            }
            return BadRequest("Invalid data");
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrder([FromBody] List<MenuItemOrderViewModel> orderedItems)
        {
            if (!User.HasClaim("Permission", Permissions.MenuManagment.Update))
                return StatusCode(403);

            foreach (var item in orderedItems)
            {
                var menuItem = await _menuItemsRepository.GetMenuItemByIdAsync(item.Id);
                if (menuItem != null)
                {
                    menuItem.ParentId = item.ParentId;
                    menuItem.Order = item.Order;
                    _menuItemsRepository.UpdateMenuItem(menuItem);
                }
            }
            return Ok();
        }

        // used for updating menu item details
        [HttpGet]
        public async Task<IActionResult> GetMenuItem(int id)
        {
            if (!User.HasClaim("Permission", Permissions.MenuManagment.Update))
                return StatusCode(403);

            var menuItem = await _menuItemsRepository.GetMenuItemByIdAsync(id);

            if (menuItem == null)
                return NotFound();

            var viewModel = new MenuItemViewModel
            {
                Id = menuItem.Id,
                Title = menuItem.Title,
                Type = menuItem.Type,
                URL = menuItem.Url,
                Icon = menuItem.Icon,
                Target = menuItem.Target,
                IsPublished = menuItem.IsPublished
            };

            return Json(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMenuItem(int id, MenuItemViewModel model)
        {
            if (!User.HasClaim("Permission", Permissions.MenuManagment.Create))
                return StatusCode(403);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingMenuItem = await _menuItemsRepository.GetMenuItemByIdAsync(id);
            if (existingMenuItem == null)
                return NotFound();

            // Update menu item properties
            existingMenuItem.Title = model.Title;
            existingMenuItem.Type = model.Type;
            existingMenuItem.Url = model.URL;
            existingMenuItem.Icon = model.Icon;
            existingMenuItem.Target = model.Target!;
            existingMenuItem.IsPublished = model.IsPublished;

            _menuItemsRepository.UpdateMenuItem(existingMenuItem);

            return Ok();
        }


        // Toggle publication status for selected menu items
        [HttpPost]
        public async Task<IActionResult> TogglePublicationStatus([FromBody] List<int> itemIds)
        {
            if (!User.HasClaim("Permission", Permissions.MenuManagment.Update))
                return StatusCode(403);

            if (itemIds == null || !itemIds.Any())
                return Json(new { success = false, message = "No menu items selected." });

            await _menuItemsRepository.ToggleMenuItemsPublicationStatusAsync(itemIds);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMenuItems([FromBody] List<int> itemIds)
        {
            if (!User.HasClaim("Permission", Permissions.MenuManagment.Delete))
                return StatusCode(403);

            if (itemIds == null || !itemIds.Any())
                return Json(new { success = false, message = "No menu items selected." });
            await _menuItemsRepository.DeleteMenuItemsAsync(itemIds);
            return Json(new { success = true });
        }


        private List<MenuItemViewModel> BuildMenuTree(List<MenuItem> allMenuItems, int? parentId = null)
        {
            return allMenuItems
                .Where(x => x.ParentId == parentId)
                .OrderBy(x => x.Order)
                .Select(x => new MenuItemViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Type = x.Type,
                    IsPublished = x.IsPublished,
                    URL = x.Url,
                    ParentId = x.ParentId,
                    Children = BuildMenuTree(allMenuItems, x.Id)
                })
                .ToList();
        }

    }
}
