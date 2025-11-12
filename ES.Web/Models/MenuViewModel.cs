namespace ES.Web.Models
{
    public class MenuViewModel
    {
        public List<MenuItemViewModel> MenuItems { get; set; } = new List<MenuItemViewModel>();
        public string CurrentPath { get; set; } = string.Empty;
    }
}
