namespace ES.Core.Entities
{
    public class OrderItemAttribute
    {
        public int Id { get; set; }
        public int? SelectedOrderItemId { get; set; } // Foreign key for SelectedAttributes
        public string? Value { get; set; }
        public decimal? Price { get; set; }
    }
}
