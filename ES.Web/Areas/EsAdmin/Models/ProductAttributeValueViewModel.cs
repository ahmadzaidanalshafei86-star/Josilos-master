namespace ES.Web.Areas.EsAdmin.Models
{
    public class ProductAttributeValueViewModel
    {
        public int Id { get; set; }
        public string Value { get; set; } = null!;
        public int Order { get; set; }
    }
}
