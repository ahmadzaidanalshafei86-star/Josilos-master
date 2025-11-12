using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class CategoryTranslationFormViewModel
    {
        public int TranslationId { get; set; }
        public int CategoryId { get; set; }
        [Required(ErrorMessage = Errors.RequiredField)]
        [Display(Name = "Category name")]
        public string Name { get; set; } = null!;
        [Display(Name = "Long description")]
        public string? LongDescription { get; set; }
        [Display(Name = "Short description")]
        public string? ShortDescription { get; set; }
        [Display(Name = "Meta description")]
        public string? MetaDescription { get; set; }
        [Display(Name = "Meta keywords")]
        public string? MetaKeywords { get; set; }

        [Display(Name = "Language")]
        [RequiredIf("TranslationId == 0", ErrorMessage = Errors.RequiredField)]
        public int? LanguageId { get; set; }
        public IEnumerable<SelectListItem>? Languages { get; set; }
    }
}
