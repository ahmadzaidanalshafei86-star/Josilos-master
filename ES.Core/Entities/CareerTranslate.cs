using System.ComponentModel.DataAnnotations;


namespace ES.Core.Entities
{
    public class CareerTranslate
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string JobTitle { get; set; } = null!;
        public string Description { get; set; } = null!;
        [MaxLength(100)]
        public string? Location { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CareerId { get; set; }
        public Career Career { get; set; } = null!;

        public int? LanguageId { get; set; }
        public Language? Language { get; set; }
    }
}
