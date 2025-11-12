using System.ComponentModel.DataAnnotations;


namespace ES.Core.Entities
{
    public class ProductLabel
    {
        public int Id { get; set; }

        [Required, StringLength(30)]
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;
        public ICollection<ProductLabelTranslate> ProductLabelTranslate { get; set; } = new List<ProductLabelTranslate>();

        // Relationship with Products
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

