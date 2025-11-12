using AKM.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ES.Core.Entities
{
    [Index(nameof(Code), IsUnique = true)]
    public class Tender
    {
        public int Id { get; set; }

        [Required, MaxLength(300)]
        public string Title { get; set; } = null!;

        [Required, MaxLength(300)]
        public string Slug { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Code { get; set; } = null!;

        [Required, MaxLength(50)]
        public string CopyPrice { get; set; } = null!;

        // Dates
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? EnvelopeOpeningDate { get; set; }
        public DateTime? LastCopyPurchaseDate { get; set; }

        public string? Details { get; set; }
        public string? PricesOffered { get; set; }

        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        // Tender materials (Many-to-Many)
        public ICollection<TenderMaterial> Materials { get; set; } = new List<TenderMaterial>();

        // Tender files (multiple)
        public ICollection<TenderFile> TenderFiles { get; set; } = new List<TenderFile>();

        // Tender other attachments (multiple, each with name + file)
        public ICollection<TenderOtherAttachment> TenderOtherAttachments { get; set; } = new List<TenderOtherAttachment>();

        // Attachments / Files
        public string? TenderImageUrl { get; set; }
        public string? TenderImageAltName { get; set; }
        public string? PricesOfferedAttachmentUrl { get; set; }
        public string? InitialAwardFileUrl { get; set; }
        public string? FinalAwardFileUrl { get; set; }

        // Flags
        public bool Publish { get; set; } = false;
        public bool PublishPricesOffered { get; set; } = false;
        public bool SpecialOfferBlink { get; set; } = false;

        // 🔹 Move to archive (instead of deleting)
        public bool MoveToArchive { get; set; } = false;

        // Blink Dates
        public DateTime? BlinkStartDate { get; set; }
        public DateTime? BlinkEndDate { get; set; }

        // Timestamps
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        // Translations
        public int LanguageId { get; set; }
        public Language Language { get; set; } = null!;

        public ICollection<TenderTranslate>? TenderTranslates { get; set; }
    }

    // 🔹 Junction table for Tender ↔ Material (Many-to-Many)
    public class TenderMaterial
    {
        public int TenderId { get; set; }
        public Tender Tender { get; set; } = null!;

        public int MaterialId { get; set; }
        public Material Material { get; set; } = null!;
    }

    // 🔹 Multiple tender files (e.g., specifications, documents)
    public class TenderFile
    {
        public int Id { get; set; }

        public int TenderId { get; set; }
        public Tender Tender { get; set; } = null!;

        [Required, MaxLength(300)]
        public string FileUrl { get; set; } = null!;

        [MaxLength(200)]
        public string? FileName { get; set; } // Optional display name
    }

    // 🔹 Multiple “other attachments” (with name + file)
    public class TenderOtherAttachment
    {
        public int Id { get; set; }

        public int TenderId { get; set; }
        public Tender Tender { get; set; } = null!;

        [Required, MaxLength(200)]
        public string Name { get; set; } = null!;

        [Required, MaxLength(300)]
        public string FileUrl { get; set; } = null!;
    }
}
