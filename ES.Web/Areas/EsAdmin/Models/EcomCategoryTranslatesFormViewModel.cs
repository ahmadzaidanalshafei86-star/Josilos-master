using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class EcomCategoryTranslatesFormViewModel
    {
        public int TranslationId { get; set; }
        public int CategoryId { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        public string Name { get; set; } = null!;
        public string? LongDescription { get; set; }
        public string? ShortDescription { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        [RequiredIf("TranslationId == 0", ErrorMessage = Errors.RequiredField)]
        public int? LanguageId { get; set; }
        public IEnumerable<SelectListItem>? Languages { get; set; }
    }
}
