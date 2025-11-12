namespace ES.Web.Areas.EsAdmin.Models
{
    public class ProductAttributeTranslateModel
    {
        public int? ProductAttributeId { get; set; }
        public string? AttributeName { get; set; }
        public string? AttributeDefaultLang { get; set; }

        public IEnumerable<ProductAttributeTranslation>? PreEnteredTranslations { get; set; }
    }
}
