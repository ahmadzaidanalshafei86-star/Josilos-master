using System.ComponentModel.DataAnnotations;

namespace ES.Core.Entities
{
    public class FormTranslation
    {
        public int Id { get; set; }
        [MaxLength(255)]
        public string TranslatedTitle { get; set; } = null!;
        public string? TranslatedDescription { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relations
        public int FormId { get; set; }
        public Form Form { get; set; } = null!;
        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;

    }
}
