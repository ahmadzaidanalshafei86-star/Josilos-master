using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace ES.Core.Entities
{
    [Index(nameof(City), IsUnique = false)]
    public class ProductDelivery
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Country { get; set; } = null!;
        [MaxLength(50)]
        public string City { get; set; } = null!;
        public decimal Price { get; set; } = 0.0m;
        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
