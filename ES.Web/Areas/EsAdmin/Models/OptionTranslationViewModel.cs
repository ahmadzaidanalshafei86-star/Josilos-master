using System.ComponentModel.DataAnnotations;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class OptionTranslationViewModel
    {
        public int OptionId { get; set; }
        public string? OriginalText { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        public string TranslatedText { get; set; } = null!;
    }
}
