namespace ES.Core.Entities
{
    public class ProductTabTranslation
    {
        public int Id { get; set; }
        public int ProductTabId { get; set; }
        public ProductTab ProductTab { get; set; } = null!;

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;

        public string TranslatedTitle { get; set; } = null!;
        public string? TranslatedContent { get; set; }
        public int Order { get; set; }


    }
}
