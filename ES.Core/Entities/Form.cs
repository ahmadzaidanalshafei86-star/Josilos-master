using System.ComponentModel.DataAnnotations;

namespace ES.Core.Entities
{
    public class Form
    {
        public int Id { get; set; }
        [MaxLength(255)]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; }

        [EmailAddress]
        [MaxLength(255)]
        public string? Email { get; set; }

        //Realtions

        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;
        public ICollection<FormField> Fields { get; set; } = new List<FormField>();
        public ICollection<FormTranslation> Translations { get; set; } = new List<FormTranslation>();

        // Navigation property for Pages (One-to-Many relationship)
        public ICollection<Page> Pages { get; set; } = new List<Page>();
    }
}
