namespace ES.Core.Entities
{
    public class ProductAttributeValueTranslation
    {
        public int Id { get; set; }
        public string TranslatedValue { get; set; } = null!;
        public int ProductAttributeValueId { get; set; }
        public ProductAttributeValue ProductAttributeValue { get; set; } = null!;
        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;

    }
}
