using System.ComponentModel.DataAnnotations;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class FieldTranslationViewModel
    {
        public int FieldId { get; set; }
        public string? OriginalText { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        public string TranslatedText { get; set; } = null!;

        public string? TranslatedFieldHint { get; set; }

        public int Order { get; set; }

        public List<OptionTranslationViewModel> Options { get; set; } = new();
    }
}
