using Microsoft.EntityFrameworkCore;

namespace ES.Core.Entities
{
    [Index(nameof(Slug), IsUnique = true)]
    public class EcomCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? LongDescription { get; set; }
        public string? ShortDescription { get; set; }
        public string? FeaturedImageUrl { get; set; }
        public string? FeaturedImageAltName { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? CoverImageAltName { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int? ParentCategoryId { get; set; }
        public EcomCategory? ParentCategory { get; set; }
        public ICollection<EcomCategory>? ChildCategories { get; set; }


        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;

        public bool IsPublished { get; set; }

        public ICollection<EcomCategoryTranslate>? CategoryTranslates { get; set; }

        // Many-to-Many: Categories ↔ Products
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    }
}
