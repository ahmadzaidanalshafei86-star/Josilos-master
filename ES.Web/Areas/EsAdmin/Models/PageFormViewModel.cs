using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class PageFormViewModel
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = Errors.RequiredField)]
        public string Title { get; set; } = null!;
        public DateTime? DateInput { get; set; }
        public string? LongDescription { get; set; }
        public string? ShortDescription { get; set; }
        public IFormFile? FeaturedImage { get; set; }
        public string? FeaturedImageUrl { get; set; }
        public IFormFile? CoverImage { get; set; }
        public string? CoverImageUrl { get; set; }

        public string? VideoURL { get; set; }

        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        // Property to store selected category ID
        [Required(ErrorMessage = Errors.RequiredField)]
        public int CategoryId { get; set; }
        public IEnumerable<SelectListItem>? Categories { get; set; }
        // Property to store selected related category IDs
        public List<int>? RelatedCategoryIds { get; set; } = new List<int>();

        public IList<IFormFile>? GalleryImages { get; set; } = new List<IFormFile>();
        public IList<GalleryImageViewModel>? ExistingGalleryImages { get; set; } = new List<GalleryImageViewModel>();

        public GalleryStyle? GalleryStyle { get; set; }
        public IEnumerable<SelectListItem>? GalleryStyles { get; set; }

        public IList<IFormFile>? PageFiles { get; set; } = new List<IFormFile>();
        public IList<FileViewModel>? ExistingFiles { get; set; } = new List<FileViewModel>();

        public bool IsPublished { get; set; } = false;
        public string? LinkTo { get; set; }
        public string? LinkToType { get; set; }
        public IEnumerable<SelectListItem>? LinkToFiles { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem>? LinkToCategories { get; set; }

        public int Order { get; set; } = 0;

        // optional link to form
        public int? FormId { get; set; }
        public IEnumerable<SelectListItem>? Forms { get; set; }

        //HIDDEN inputs
        public bool KeepCoverImage { get; set; } = true;
        public bool KeepFeaturedImage { get; set; } = true;
        public IList<int>? DisabledOrderCategories { get; set; } = new List<int>();

        public int Count { get; set; }
    }
}
