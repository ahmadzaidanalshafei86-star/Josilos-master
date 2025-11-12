using System.ComponentModel.DataAnnotations;

namespace ES.Core.Entities
{
    public class Career
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string? RefNumber { get; set; }
        [MaxLength(100)]
        public string JobTitle { get; set; } = null!;
        public string Description { get; set; } = null!;
        [MaxLength(100)]
        public string? Location { get; set; }
        public decimal? Salary { get; set; }

        [MaxLength(100)]
        public string? EmploymentType { get; set; }

        [MaxLength(50)]
        public string? EnviromentType { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;


        public ICollection<CareerTranslate> CareerTranslates { get; set; } = new List<CareerTranslate>();
        public ICollection<CareerApplication> Applications { get; set; } = new List<CareerApplication>(); // Link to career applications
    }
}
