namespace ES.Web.Areas.EsAdmin.Models
{
    public class BrandTranslateViewModel
    {
        public int? BrandId { get; set; }
        public string? BrandName { get; set; }
        public DateTime CreatedDate { get; set; }

        public string? BrandDefaultLang { get; set; }

        public IEnumerable<BrandTranslate>? PreEnteredTranslations { get; set; }
    }
}
