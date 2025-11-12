namespace ES.Web.Areas.EsAdmin.Models
{
    public class ProductLabelTranslateModel
    {
        public int? ProductLabelId { get; set; }
        public string? ProductLabelName { get; set; }
        public string? ProductLabelDefaultLang { get; set; }

        public DateTime CreatedAt { get; set; }

        public IEnumerable<ProductLabelTranslate>? PreEnteredTranslations { get; set; }
    }
}
