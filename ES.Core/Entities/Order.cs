using System.ComponentModel.DataAnnotations;

namespace ES.Core.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerFullName { get; set; } = null!;
        public string? Email { get; set; }
        public string MobilePhone { get; set; } = null!;
        public string StreetAddress { get; set; } = null!;
        [MaxLength(50)]
        public string Country { get; set; } = string.Empty;
        [MaxLength(50)]
        public string City { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = null!;
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal EstimatedTotal { get; set; }
        public bool IsDelivered { get; set; } = false;
        public bool IsCancelled { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? OrderComment { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
