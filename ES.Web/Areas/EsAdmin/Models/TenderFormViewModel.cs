using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class TenderFormViewModel
    {
        public int? Id { get; set; }

        // 🔹 Basic Info
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Slug { get; set; }

        [Required(ErrorMessage = "Code is required")]
        [MaxLength(100)]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Copy Price is required")]
        [MaxLength(50)]
        public string CopyPrice { get; set; } = string.Empty;

        // 🔹 Dates
        [Required(ErrorMessage = "Start Date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required")]
        public DateTime EndDate { get; set; }

        public DateTime? EnvelopeOpeningDate { get; set; }
        public DateTime? LastCopyPurchaseDate { get; set; }

        // 🔹 Content
        public string? Details { get; set; }
        public string? PricesOffered { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        // 🔹 Materials
        public List<int>? SelectedMaterialIds { get; set; } = new();
        public IEnumerable<SelectListItem>? Materials { get; set; }

        // 🔹 File Attachments
        public IList<IFormFile>? TenderFiles { get; set; } = new List<IFormFile>();
        public IList<TenderFileViewModel>? ExistingTenderFiles { get; set; } = new List<TenderFileViewModel>();

        // 🔹 Other Attachments
        public IList<TenderOtherAttachmentViewModel>? OtherAttachments { get; set; } = new List<TenderOtherAttachmentViewModel>();

        // 🔹 Uploads
        public IFormFile? TenderImage { get; set; }
        public string? TenderImageUrl { get; set; }

        public IFormFile? PricesOfferedAttachment { get; set; }
        public string? PricesOfferedAttachmentUrl { get; set; }

        public IFormFile? InitialAwardFile { get; set; }
        public string? InitialAwardFileUrl { get; set; }

        public IFormFile? FinalAwardFile { get; set; }
        public string? FinalAwardFileUrl { get; set; }

        // 🔹 Flags
        public bool Publish { get; set; }
        public bool PublishPricesOffered { get; set; }
        public bool SpecialOfferBlink { get; set; }
        public bool MoveToArchive { get; set; }

        // 🔹 Blink Duration
        public DateTime? BlinkStartDate { get; set; }
        public DateTime? BlinkEndDate { get; set; }

        // 🔹 Language
        public int? LanguageId { get; set; }
        public IEnumerable<SelectListItem>? Languages { get; set; }

        // 🔹 Dropdown Data
        public IEnumerable<SelectListItem>? Tenders { get; set; }
        public IEnumerable<SelectListItem>? SortingTypes { get; set; }

        // 🔹 System Fields
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public int Count { get; set; }

        // 🔹 Extra Attachments
        public List<TenderOtherAttachmentViewModel> TenderOtherAttachments { get; set; } = new();
    }

    public class TenderFileViewModel
    {
        public int? Id { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
    }

    public class TenderOtherAttachmentViewModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public IFormFile? File { get; set; }
        public string? FileUrl { get; set; }
    }
}
