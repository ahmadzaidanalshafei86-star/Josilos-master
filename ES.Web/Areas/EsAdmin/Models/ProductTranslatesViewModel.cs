namespace ES.Web.Areas.EsAdmin.Models
{
    public class ProductTranslatesViewModel
    {
        public int? ProductId { get; set; }
        public string? ProductTitle { get; set; }
        public DateTime CreatedDate { get; set; }

        public string? ProductDefaultLang { get; set; }

        public IEnumerable<ProductTranslate>? PreEnteredTranslations { get; set; }
    }
}
