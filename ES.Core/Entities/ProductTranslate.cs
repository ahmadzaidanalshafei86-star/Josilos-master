namespace ES.Core.Entities
{
    public class ProductTranslate
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? LongDescription { get; set; }
        public string? ShortDescription { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;
    }
}
