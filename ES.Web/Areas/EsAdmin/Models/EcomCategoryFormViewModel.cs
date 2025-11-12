using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class EcomCategoryFormViewModel
    {
        public int Id { get; set; }

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

        // Property to store selected parent category ID
        public int? ParentCategoryId { get; set; }
        public IEnumerable<SelectListItem>? Categories { get; set; }

        public bool IsPublished { get; set; } = false;

        //HIDDEN inputs
        public bool KeepCoverImage { get; set; } = true;
        public bool KeepFeaturedImage { get; set; } = true;
    }
}
