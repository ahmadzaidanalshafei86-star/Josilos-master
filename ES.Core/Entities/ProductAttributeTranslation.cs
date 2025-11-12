using System.ComponentModel.DataAnnotations;

namespace ES.Core.Entities
{
    public class ProductAttributeTranslation
    {
        public int Id { get; set; }
        public int ProductAttributeId { get; set; }

        [MaxLength(100)]
        public string TranslatedName { get; set; } = null!;

        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;


        public ProductAttribute ProductAttribute { get; set; } = null!;
    }
}
