using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace ES.Core.Entities
{
    [Index(nameof(Slug), IsUnique = true)]
    public class Brand
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;

        public string? LogoAltName { get; set; } = null!;
        public string? LogoUrl { get; set; }

        public bool IsActive { get; set; } = true; // To enable/disable a brand

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;
        public ICollection<BrandTranslate> BrandTranslates { get; set; } = new List<BrandTranslate>();

        // Relationship with Products
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
