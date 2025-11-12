namespace ES.Web.Areas.EsAdmin.Models
{
    public class ProductAttributeMappingViewModel
    {
        public int AttributeId { get; set; }
        public string? AttributeName { get; set; }

        public List<ProductAttributeValueFormModel> Values { get; set; } = new();
    }
}
