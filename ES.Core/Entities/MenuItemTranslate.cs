using System.ComponentModel.DataAnnotations;


namespace ES.Core.Entities
{
    public class MenuItemTranslate
    {
        public int Id { get; set; }

        public int MenuItemId { get; set; }
        public MenuItem? MenuItem { get; set; }

        public int? LanguageId { get; set; }
        public Language? Language { get; set; }

        [StringLength(255)]
        public string Title { get; set; } = null!;// Translated title

        [StringLength(500)]
        public string? CustomUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
