namespace ES.Core.Entities
{
    public class ProductAttributeValue
    {
        public int Id { get; set; }

        public string? Value { get; set; }
        public int Order { get; set; } // Ensures values are ordered

        public int ProductAttributeId { get; set; }
        public ProductAttribute ProductAttribute { get; set; } = null!;

        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;


        public ICollection<ProductAttributeValueTranslation> Translations { get; set; } = new List<ProductAttributeValueTranslation>();
    }
}
