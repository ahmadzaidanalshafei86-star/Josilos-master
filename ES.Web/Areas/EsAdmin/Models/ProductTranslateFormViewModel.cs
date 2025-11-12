using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class ProductTranslateFormViewModel
    {
        public int TranslationId { get; set; }
        public int ProductId { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        public string Title { get; set; } = null!;
        public string? LongDescription { get; set; }
        public string? ShortDescription { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        public IList<ProductTabViewModel> ProductTabs { get; set; } = new List<ProductTabViewModel>();

        [RequiredIf("TranslationId == 0", ErrorMessage = Errors.RequiredField)]
        public int? LanguageId { get; set; }
        public IEnumerable<SelectListItem>? Languages { get; set; }
    }
}
