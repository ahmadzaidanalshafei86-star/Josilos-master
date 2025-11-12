using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class ProductAttributeTranslateFormModel
    {
        public int TranslationId { get; set; }
        public int AttributeId { get; set; }

        public string? Name { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        public string TranslatedName { get; set; } = null!;
        public List<ProductAttributeValueTranslateModel> Values { get; set; } = new();

        [RequiredIf("TranslationId == 0", ErrorMessage = Errors.RequiredField)]
        public int? LanguageId { get; set; }
        public IEnumerable<SelectListItem>? Languages { get; set; }
    }
}
