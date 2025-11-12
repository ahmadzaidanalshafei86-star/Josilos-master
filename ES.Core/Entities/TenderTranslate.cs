using ES.Core.Entities;

namespace AKM.Core.Entities
{
    public class TenderTranslate
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;
        public string? Details { get; set; }
        public string? PricesOffered { get; set; }


        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public int TenderId { get; set; }
        public Tender Tender { get; set; } = null!;

        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;
    }
}
