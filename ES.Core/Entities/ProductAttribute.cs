using System.ComponentModel.DataAnnotations;

namespace ES.Core.Entities
{
    public class ProductAttribute
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;

        public ICollection<ProductAttributeTranslation> Translations { get; set; } = new List<ProductAttributeTranslation>();
        public ICollection<ProductAttributeValue> Values { get; set; } = new List<ProductAttributeValue>();
    }
}
