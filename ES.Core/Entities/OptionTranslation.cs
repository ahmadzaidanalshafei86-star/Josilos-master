namespace ES.Core.Entities
{
    public class OptionTranslation
    {
        public int Id { get; set; }
        public string TranslatedText { get; set; } = string.Empty;

        // Relations
        public int OptionId { get; set; }
        public FormOption Option { get; set; } = null!;
        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;
    }
}
