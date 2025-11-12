using Microsoft.EntityFrameworkCore;
using ES.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace ES.Core.Entities
{
    [Index(nameof(Slug), IsUnique = true)]
    public class Product
    {
        public int Id { get; set; }

        // Basic Product Info
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? CoverImageAltName { get; set; }
        public string? FeaturedImageUrl { get; set; }
        public string? FeaturedImageAltName { get; set; }
        public string? VideoUrl { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Meta for SEO
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        // Many-to-Many: Products ↔ Categories
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }

        public int? ProductLabelId { get; set; }
        public ProductLabel? ProductLabel { get; set; }

        // Foreign Key to Language
        public int LanguageId { get; set; }
        public Language? Language { get; set; }

        // Collections for Gallery and Translations
        public ICollection<ProductGalleryImage>? ProductGalleryImages { get; set; }
        public ICollection<ProductTranslate>? ProductTranslates { get; set; }

        // Product Type (Simple or Variable)
        public ProductType ProductType { get; set; }

        // Pricing Tab (Only for Simple Products)
        public decimal? RegularPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public DateTime? SaleStartDate { get; set; }
        public DateTime? SaleEndDate { get; set; }

        // Inventory Tab (Only for Simple Products)
        public string? SKU { get; set; }
        public string? GTIN { get; set; }

        public bool ManageStock { get; set; } = false; // If true, track quantity
        public int? StockQuantity { get; set; } // Available stock

        [MaxLength(50)]
        public BackorderStatus AllowBackorders { get; set; }
        public int LowStockThreshold { get; set; } = 2; // Default threshold

        [MaxLength(200)]
        public string? StockStatus { get; set; }

        public bool SoldIndividually { get; set; } = false; // Limit purchase to 1 per order
        public bool ExcludeSoldOutBadge { get; set; } = false; // Hide "Sold Out" badge

        public bool IsPublished { get; set; }

        // linked products

        // Self-referencing many-to-many relationship for linked products
        public ICollection<ProductLinked> LinkedProducts { get; set; } = new List<ProductLinked>();
        public ICollection<ProductLinked> LinkedToProducts { get; set; } = new List<ProductLinked>();

        // Product Attributes
        public ICollection<ProductAttributeMapping> ProductAttributes { get; set; } = new List<ProductAttributeMapping>();

        // product Tabs ( optional )
        public bool DescriptionTab { get; set; }
        public bool ReviewTab { get; set; }
        public ICollection<ProductTab> ProductTabs { get; set; } = new List<ProductTab>();

        public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
    }
}
