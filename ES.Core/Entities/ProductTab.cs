namespace ES.Core.Entities
{
    public class ProductTab
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Content { get; set; } // text editor


        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;

        public int Order { get; set; }

        public ICollection<ProductTabTranslation> Translations { get; set; } = new List<ProductTabTranslation>();
    }
}
