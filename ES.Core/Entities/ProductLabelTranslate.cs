using System.ComponentModel.DataAnnotations;

namespace ES.Core.Entities
{
    public class ProductLabelTranslate
    {
        public int Id { get; set; }
        [Required, StringLength(30)]
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int ProductLabelId { get; set; }
        public ProductLabel ProductLabel { get; set; } = null!;
        public int? LanguageId { get; set; }
        public Language? Language { get; set; }
    }
}
