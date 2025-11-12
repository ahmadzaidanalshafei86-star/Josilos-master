namespace ES.Web.Areas.EsAdmin.Models
{
    public class PageTranslatesViewModel
    {
        public int? PageId { get; set; }
        public string? PageTitle { get; set; }
        public DateTime CreatedDate { get; set; }

        public string? PageDefaultLang { get; set; }

        public IEnumerable<PageTranslate>? PreEnteredTranslations { get; set; }
    }
}
