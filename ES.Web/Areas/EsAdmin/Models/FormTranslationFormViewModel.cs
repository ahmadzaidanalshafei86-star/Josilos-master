using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class FormTranslationFormViewModel
    {
        public int TranslationId { get; set; }
        public int FormId { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public List<FieldTranslationViewModel> Fields { get; set; } = new();

        [RequiredIf("TranslationId == 0", ErrorMessage = Errors.RequiredField)]
        public int? LanguageId { get; set; }
        public IEnumerable<SelectListItem>? Languages { get; set; }
    }
}
