using Microsoft.EntityFrameworkCore;
using ES.Core.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ES.Core.Entities
{
    [Index(nameof(Slug), IsUnique = true)]
    public class Category
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
        public int Order { get; set; } = 0;

        public TypeOfSorting TypeOfSorting { get; set; }
        public GalleryStyle? GalleryStyle { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [ForeignKey(nameof(ParentCategory))]
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
        public ICollection<Category>? ChildCategories { get; set; }

        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;

        public int ThemeId { get; set; }
        public Theme Theme { get; set; } = null!;

        [DefaultValue(false)]
        public bool IsPublished { get; set; }
        public string? Link { get; set; }

        public ICollection<GalleryImage>? GalleryImages { get; set; }
        public ICollection<CategoryTranslate>? CategoryTranslates { get; set; }


        // Related Categories (Many-to-Many relationship)
        public ICollection<Category>? RelatedCategories { get; set; } = new List<Category>();
        public ICollection<Category>? CategoriesRelatedToThis { get; set; } = new List<Category>();

        //Nav property to pages of this category
        public ICollection<Page> PagesRelatedToThis { get; set; } = new List<Page>();

        // This collection represents the related pages for this category.
        // A category can have many pages, and the PageCategory table manages this relationship.
        //used in the multiSelection dropdown of related categories in the page form 
        public ICollection<PageCategory>? PageCategories { get; set; }
    }
}
