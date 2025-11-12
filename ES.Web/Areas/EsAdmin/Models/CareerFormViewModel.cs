using System.ComponentModel.DataAnnotations;


namespace ES.Web.Areas.EsAdmin.Models
{
    public class CareerFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        [StringLength(100, ErrorMessage = Errors.MaxLength)]
        public string JobTitle { get; set; } = null!;

        [StringLength(50, ErrorMessage = Errors.MaxLength)]
        public string? RefNumber { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        public string Description { get; set; } = null!;

        [StringLength(100, ErrorMessage = Errors.MaxLength)]
        public string? Location { get; set; }

        public string? EmploymentType { get; set; }
        public string? EnviromentType { get; set; }

        [Range(0, 9999999.99, ErrorMessage = "Salary must be between 0 and 9999,999.99")]
        public decimal? Salary { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
