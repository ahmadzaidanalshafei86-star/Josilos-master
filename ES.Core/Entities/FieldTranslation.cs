namespace ES.Core.Entities
{
    public class FieldTranslation
    {
        public int Id { get; set; }
        public string TranslatedText { get; set; } = null!;
        public string? TranslatedFieldHint { get; set; }
        public int FieldId { get; set; }
        public FormField Field { get; set; } = null!;
        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;
    }
}
