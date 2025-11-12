using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class CategoryFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        public string Name { get; set; } = null!;

        public string? LongDescription { get; set; }
        public string? ShortDescription { get; set; }

        public IFormFile? FeaturedImage { get; set; }
        public string? FeaturedImageUrl { get; set; }

        public IFormFile? CoverImage { get; set; }
        public string? CoverImageUrl { get; set; }

        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        public TypeOfSorting TypeOfSorting { get; set; }
        public IEnumerable<SelectListItem>? SortingTypes { get; set; }

        public int Order { get; set; } = 0;

        public int ThemeId { get; set; }
        public IEnumerable<SelectListItem>? Themes { get; set; }

        public int? ParentCategoryId { get; set; }

        // Property to store selected related category IDs
        public List<int>? RelatedCategoryIds { get; set; } = new List<int>();
        public IEnumerable<SelectListItem>? Categories { get; set; }

        public IList<IFormFile>? GalleryImages { get; set; } = new List<IFormFile>();
        public IList<GalleryImageViewModel>? ExistingGalleryImages { get; set; } = new List<GalleryImageViewModel>();

        public GalleryStyle? GalleryStyle { get; set; }
        public IEnumerable<SelectListItem>? GalleryStyles { get; set; }

        public bool IsPublished { get; set; } = false;
        public string? Link { get; set; }

        //HIDDEN inputs
        public bool KeepCoverImage { get; set; } = true;
        public bool KeepFeaturedImage { get; set; } = true;
    }
}
