namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class MenuItemsRepository
    {
        private readonly ApplicationDbContext _context;

        public MenuItemsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuItem>> GetAllMenuItemsAsync()
        {
            return await _context.MenuItems
                .Include(m => m.Parent)
                .ToListAsync();
        }

        public async Task<MenuItem> GetMenuItemByIdAsync(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);

            if (menuItem is null)
                throw new Exception("Menu item not found");

            return menuItem;
        }
        public async Task<int> AddMenuItemAsync(MenuItem menuItem)
        {
            await _context.MenuItems.AddAsync(menuItem);
            await _context.SaveChangesAsync();
            return menuItem.Id;
        }

        public void UpdateMenuItem(MenuItem menuItem)
        {
            _context.MenuItems.Update(menuItem);
            _context.SaveChanges();
        }

        public async Task DeleteMenuItemsAsync(List<int> itemIds)
        {
            var menuItems = await _context.MenuItems
                                          .Where(mi => itemIds.Contains(mi.Id))
                                          .ToListAsync();

            // Find all child items that have a parent in the list of items to be deleted
            var childItems = await _context.MenuItems
                                           .Where(mi => itemIds.Contains(mi.ParentId.Value))
                                           .ToListAsync();

            // Set their ParentId to null (detach)
            foreach (var child in childItems)
                child.ParentId = null;

            _context.MenuItems.RemoveRange(menuItems);
            await _context.SaveChangesAsync();
        }


        public async Task ToggleMenuItemsPublicationStatusAsync(List<int> itemIds)
        {
            var menuItems = await _context.MenuItems.Where(mi => itemIds.Contains(mi.Id)).ToListAsync();

            foreach (var item in menuItems)
            {
                item.IsPublished = !item.IsPublished;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<MenuItem> GetMenuItemWithTranslationsAsync(int menuItemId)
        {
            var menuItem = await _context.MenuItems
                 .Include(c => c.Language)
                 .Include(c => c.Translations!)
                 .ThenInclude(c => c.Language)
                 .SingleOrDefaultAsync(c => c.Id == menuItemId);

            if (menuItem == null)
                throw new Exception(message: "Menu item not found");

            return menuItem;
        }


    }
}
