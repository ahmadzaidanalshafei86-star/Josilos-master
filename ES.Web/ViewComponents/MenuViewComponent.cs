using ES.Web.Models;
using ES.Web.Services;

namespace ES.Web.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly MenuItemsService _menuItemsService;

        public MenuViewComponent(MenuItemsService menuItemsService)
        {
            _menuItemsService = menuItemsService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var menuItems = await _menuItemsService.GetMenuItemsAsync();

            var menuItemViewModels = menuItems.Select(MapMenuItem).ToList();

            MenuViewModel model = new()
            {
                MenuItems = menuItemViewModels,
                CurrentPath = HttpContext.Request.Path.Value ?? string.Empty
            };

            return View(model);
        }

        private MenuItemViewModel MapMenuItem(MenuItem menuItem)
        {
            var translation = menuItem.Translations.FirstOrDefault();

            return new MenuItemViewModel
            {
                Id = menuItem.Id,
                Title = translation?.Title ?? menuItem.Title,
                Url = menuItem.Type == "CustomLink" ? translation?.CustomUrl ?? menuItem.Url : menuItem.Url,
                Icon = menuItem.Icon,
                Type = menuItem.Type,
                Target = menuItem.Target,
                Children = menuItem.Children?.Select(MapMenuItem).ToList() ?? new List<MenuItemViewModel>()
            };
        }
    }
}
