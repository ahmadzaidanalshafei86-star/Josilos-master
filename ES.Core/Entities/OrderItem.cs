namespace ES.Core.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductTitle { get; set; } = string.Empty;
        public string? CustomizationName { get; set; }
        public string? FeaturedImageUrl { get; set; }
        public int Quantity { get; set; }
        public List<OrderItemAttribute> SelectedAttributes { get; set; } = new List<OrderItemAttribute>();
    }
}
