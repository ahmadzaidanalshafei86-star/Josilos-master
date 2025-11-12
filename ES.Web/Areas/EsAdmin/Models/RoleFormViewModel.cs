using System.ComponentModel.DataAnnotations;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class RoleFormViewModel
    {
        public string? Id { get; set; }

        [MaxLength(50, ErrorMessage = Errors.MaxLength)]
        public string RoleName { get; set; } = null!;
    }
}
