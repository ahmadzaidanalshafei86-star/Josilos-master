using ES.Web.Helpers;

namespace ES.Web.Services
{
    public class MenuItemsService
    {
        private readonly ApplicationDbContext _context;
        public MenuItemsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuItem>> GetMenuItemsAsync()
        {
            var languageId = await LanguageHelper.GetCurrentLanguageIdAsync(_context);

            // Fetch top-level menu items (ParentId == null) with translations
            var topLevelItems = await _context.MenuItems
                .AsNoTracking()
                .Where(m => m.ParentId == null && m.IsPublished)
                .OrderBy(m => m.Order)
                .Include(m => m.Translations.Where(t => t.LanguageId == languageId))
                .ToListAsync();

            // Recursively load children for each top-level item
            foreach (var item in topLevelItems)
            {
                await LoadChildrenAsync(item, languageId);
            }

            return topLevelItems;
        }

        private async Task LoadChildrenAsync(MenuItem menuItem, int? languageId)
        {
            // Fetch children for the current menu item
            var children = await _context.MenuItems
                .AsNoTracking()
                .Where(m => m.ParentId == menuItem.Id && m.IsPublished)
                .OrderBy(m => m.Order)
                .Include(m => m.Translations.Where(t => t.LanguageId == languageId))
                .ToListAsync();

            // Assign children to the menu item
            menuItem.Children = children;

            // Recursively load children for each child
            foreach (var child in children)
            {
                await LoadChildrenAsync(child, languageId);
            }
        }
    }
}
