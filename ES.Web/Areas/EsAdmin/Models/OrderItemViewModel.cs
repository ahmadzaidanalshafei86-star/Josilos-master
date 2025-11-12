namespace ES.Web.Areas.EsAdmin.Models
{
    public class OrderItemViewModel
    {
        public string ProductTitle { get; set; } = null!;
        public string? CustomizationName { get; set; }
        public int Quantity { get; set; }
        public List<OrderItemAttributeViewModel> SelectedAttributes { get; set; } = new List<OrderItemAttributeViewModel>();
    }
}
