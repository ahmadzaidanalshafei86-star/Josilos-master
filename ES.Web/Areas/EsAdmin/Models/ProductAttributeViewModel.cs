namespace ES.Web.Areas.EsAdmin.Models
{
    public class ProductAttributeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<ProductAttributeValueViewModel> Values { get; set; } = new();
    }
}
