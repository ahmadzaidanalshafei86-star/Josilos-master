namespace ES.Core.Entities
{
    public class EcomCategoryTranslate
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? LongDescription { get; set; }
        public string? ShortDescription { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int EcomCategoryId { get; set; }
        public EcomCategory EcomCategory { get; set; } = null!;

        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;
    }
}
