using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ES.Core.Entities
{
    public class MenuItem
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public MenuItem? Parent { get; set; }
        public ICollection<MenuItem> Children { get; set; } = new List<MenuItem>();

        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;

        [StringLength(255)]
        public string Title { get; set; } = null!;

        [StringLength(20)]
        public string Target { get; set; } = null!;

        [StringLength(50)]
        public string Type { get; set; } = null!; // "Page", "Category", "CustomLink", "Careers", "HomePage", "ProductCategory", "BlankLink"

        [StringLength(500)]
        public string? Url { get; set; } //URL or slug 

        public int Order { get; set; } = 0;
        public bool IsPublished { get; set; } = false;

        [StringLength(100)]
        public string? Icon { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public ICollection<MenuItemTranslate> Translations { get; set; } = new List<MenuItemTranslate>();

        [NotMapped]
        public string? ImageUrl { get; set; } // Used for EcomCategory
    }
}
