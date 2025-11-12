using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class CareerTranslatesFormViewModel
    {
        public int TranslationId { get; set; }
        public int CareerId { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        [StringLength(100, ErrorMessage = Errors.MaxLength)]
        public string JobTitle { get; set; } = null!;

        [Required(ErrorMessage = Errors.RequiredField)]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = Errors.RequiredField)]
        [StringLength(100, ErrorMessage = Errors.MaxLength)]
        public string Location { get; set; } = null!;

        [RequiredIf("TranslationId == 0", ErrorMessage = Errors.RequiredField)]
        public int? LanguageId { get; set; }
        public IEnumerable<SelectListItem>? Languages { get; set; }
    }
}
