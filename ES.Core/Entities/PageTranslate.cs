namespace ES.Core.Entities
{
    public class PageTranslate
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? LongDescription { get; set; }
        public string? ShortDescription { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public DateTime CreatedDate { get; set; }

        public int PageId { get; set; }
        public Page Page { get; set; } = null!;

        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;
    }
}
