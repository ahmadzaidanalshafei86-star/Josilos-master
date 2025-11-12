namespace ES.Web.Areas.EsAdmin.Models
{
    public class MenuItemTranslatesViewModel
    {
        public int? MenuItemId { get; set; }
        public string? MenuItemTitle { get; set; }
        public DateTime CreatedDate { get; set; }

        public string? MenuItemDefaultLang { get; set; }

        public IEnumerable<MenuItemTranslate>? PreEnteredTranslations { get; set; }
    }
}
