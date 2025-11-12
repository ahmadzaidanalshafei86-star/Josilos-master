


using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class MaterialTranslationFormViewModel
    {
        public int TranslationId { get; set; }
        public int MaterialId { get; set; }
        [Required(ErrorMessage = Errors.RequiredField)]
        [Display(Name = "Material name")]
        public string Name { get; set; } = null!;
      

        [Display(Name = "Language")]
        [RequiredIf("TranslationId == 0", ErrorMessage = Errors.RequiredField)]
        public int? LanguageId { get; set; }
        public IEnumerable<SelectListItem>? Languages { get; set; }
    }
}
