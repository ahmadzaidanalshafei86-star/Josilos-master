namespace ES.Web.Areas.EsAdmin.Models
{
    public class LanguageSelectionViewModel
    {
        public string? SelectedLanguageCode { get; set; } // Stores the selected language Code
        public IList<Language>? Languages { get; set; } // List of available languages
    }
}
