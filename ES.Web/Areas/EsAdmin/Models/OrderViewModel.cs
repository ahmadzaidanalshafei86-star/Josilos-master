using System.ComponentModel.DataAnnotations;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class OrderViewModel
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
        public string? OrderComment { get; set; }
        public bool IsDelivered { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
    }
}
