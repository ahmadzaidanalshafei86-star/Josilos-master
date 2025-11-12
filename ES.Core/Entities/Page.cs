using Microsoft.EntityFrameworkCore;
using ES.Core.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ES.Core.Entities
{
    [Index(nameof(Slug), IsUnique = true)]
    public class Page
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? URLTarget { get; set; }
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? CoverImageAltName { get; set; }
        public string? FeatruedImageUrl { get; set; }
        public string? FeaturedImageAltName { get; set; }
        public int Order { get; set; }

        public string? VideoURL { get; set; }

        //meta for SEO
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        // Foreign Key to category
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // Related Categories (One-to-Many relationship)
        public ICollection<PageCategory>? RelatedCategories { get; set; }


        // Foreign Key to Language
        public int LanguageId { get; set; }
        public Language? Language { get; set; }

        // Audit Properties
        public DateTime CreatedDate { get; set; }
        public DateTime? DateInput { get; set; }

        public string LinkTo { get; set; } = null!;
        [MaxLength(50)]
        public string LinkToType { get; set; } = null!;
        public string LinkToUrl { get; set; } = null!;

        [DefaultValue(false)]
        public bool IsPublished { get; set; }

        public int? FormId { get; set; }
        public Form? Form { get; set; }


        public GalleryStyle? GalleryStyle { get; set; }
        public ICollection<PageGalleryImage>? PageGalleryImages { get; set; }
        public ICollection<PageFile>? PageFiles { get; set; }
        public ICollection<PageTranslate>? PageTranslates { get; set; }

        public int Count { get; set; } = 0;
    }
}
