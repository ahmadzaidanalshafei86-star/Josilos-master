namespace ES.Web.Models
{
    public class MenuItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Type { get; set; }
        public string? Target { get; set; }
        public string? Url { get; set; }

        public List<MenuItemViewModel> Children { get; set; } = new List<MenuItemViewModel>();
    }
}
