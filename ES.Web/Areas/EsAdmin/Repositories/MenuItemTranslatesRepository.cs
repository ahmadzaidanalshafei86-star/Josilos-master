using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Web.Areas.EsAdmin.Models;

namespace ES.Web.Areas.EsAdmin.Repositories
{
    public class MenuItemTranslatesRepository
    {
        private readonly ApplicationDbContext _context;
        public MenuItemTranslatesRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<MenuItemTranslate> GetMenuItemTranslateByIdAsync(int id)
        {
            var translate = await _context.MenuItemTranslates.FindAsync(id);

            if (translate == null)
                throw new Exception(message: "Menu item not found");

            return translate;
        }

        public async Task<MenuItemTranslationFormViewModel> InitializeMenuItemTranslatesFormViewModelAsync(int menuItemId, MenuItemTranslationFormViewModel? model = null)
        {
            model ??= new MenuItemTranslationFormViewModel(); // Initialize a new model if none is provided.

            model.Languages = await GetLanguagesAsync(menuItemId);
            return model;
        }
        public async Task<int> AddMenuItemTranslateAsync(MenuItemTranslate menuItemTranslate)
        {
            await _context.MenuItemTranslates.AddAsync(menuItemTranslate);
            await _context.SaveChangesAsync();
            return menuItemTranslate.Id;
        }

        public async Task<bool> DeleteTranslationAsync(int id)
        {
            var translate = await GetMenuItemTranslateByIdAsync(id);

            _context.MenuItemTranslates.Remove(translate);
            await _context.SaveChangesAsync();

            return true;
        }

        public void UpdateTranslate(MenuItemTranslate menuItemTranslate)
        {
            _context.MenuItemTranslates.Update(menuItemTranslate);
            _context.SaveChanges();
        }

        private async Task<IEnumerable<SelectListItem>> GetLanguagesAsync(int MenuItemId)
        {
            // Get all language IDs that already have translations for the given MenuItem
            var translatedLanguageIds = await _context.MenuItemTranslates
                .Where(ct => ct.MenuItemId == MenuItemId)
                .Select(ct => ct.LanguageId)
                .ToListAsync();

            // to know the default lang of the menu and not showing it in the dropdown
            var menuItem = await _context.MenuItems
                .Include(c => c.Language)
                .SingleOrDefaultAsync(c => c.Id == MenuItemId);

            if (menuItem == null)
                throw new Exception(message: "Menu item not found");

            return await _context.Languages
           .Where(l => l.Code != menuItem.Language.Code && !translatedLanguageIds.Contains(l.Id))
          .Select(th => new SelectListItem
          {
              Value = th.Id.ToString(),
              Text = th.Name
          })
          .ToListAsync();
        }
    }
}
