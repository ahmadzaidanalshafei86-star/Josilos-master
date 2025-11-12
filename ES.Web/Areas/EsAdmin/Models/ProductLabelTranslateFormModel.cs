using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class ProductLabelTranslateFormModel
    {
        public int TranslationId { get; set; }
        public int ProductLabelId { get; set; }
        [Required(ErrorMessage = Errors.RequiredField)]
        public string Name { get; set; } = null!;
        [RequiredIf("TranslationId == 0", ErrorMessage = Errors.RequiredField)]
        public int? LanguageId { get; set; }
        public IEnumerable<SelectListItem>? Languages { get; set; }
    }
}
