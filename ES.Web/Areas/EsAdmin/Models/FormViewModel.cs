using System.ComponentModel.DataAnnotations;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class FormViewModel
    {
        public int Id { get; set; }
        [MinLength(1)]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        [EmailAddress(ErrorMessage = Errors.Email)]
        public string? Email { get; set; }
        public List<FormFieldViewModel> Fields { get; set; } = new();
    }
}
