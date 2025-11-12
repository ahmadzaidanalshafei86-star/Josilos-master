using System.ComponentModel.DataAnnotations;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class ProductAttributeValueTranslateModel
    {
        public int ValueId { get; set; }
        public string? OriginalText { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        public string TranslatedText { get; set; } = null!;

        public int Order { get; set; }
    }
}
