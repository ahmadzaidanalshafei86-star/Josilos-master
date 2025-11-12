using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class MenuItemTranslationFormViewModel
    {

        public int TranslationId { get; set; }
        public int MenuItemId { get; set; }
        [Required(ErrorMessage = Errors.RequiredField)]
        public string Title { get; set; } = null!;
        [MaxLength(450, ErrorMessage = Errors.MaxLength)]
        public string? CustomUrl { get; set; }
        [RequiredIf("TranslationId == 0", ErrorMessage = Errors.RequiredField)]
        public int? LanguageId { get; set; }
        public IEnumerable<SelectListItem>? Languages { get; set; }
    }
}
