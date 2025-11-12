using System.ComponentModel.DataAnnotations;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class BrandFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        [Remote("AllowBrandName", null!, AdditionalFields = "Id", ErrorMessage = Errors.Duplicated)]
        public string Name { get; set; } = null!;

        [RequiredIf("Id == 0", ErrorMessage = Errors.RequiredField)]
        public IFormFile? Logo { get; set; }

        public string? LogoUrl { get; set; }
        public string? LogoAltName { get; set; }
        public bool IsActive { get; set; }
    }
}
