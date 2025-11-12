using Microsoft.AspNetCore.Mvc.Rendering;
using ES.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class ProductFormViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = Errors.RequiredField)]
        public string Title { get; set; } = null!;
        public string? LongDescription { get; set; }
        public string? ShortDescription { get; set; }
        public IFormFile? FeaturedImage { get; set; }
        public string? FeaturedImageUrl { get; set; }
        public IFormFile? CoverImage { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        [Required(ErrorMessage = Errors.RequiredField)]
        public List<int> CategoryIds { get; set; } = null!;
        public IEnumerable<SelectListItem>? Categories { get; set; }


        public int? BrandId { get; set; }
        public IEnumerable<SelectListItem>? Brands { get; set; }

        public int? LabelId { get; set; }
        public IEnumerable<SelectListItem>? Labels { get; set; }


        public IList<IFormFile>? GalleryImages { get; set; } = new List<IFormFile>();
        public IList<GalleryImageViewModel>? ExistingGalleryImages { get; set; } = new List<GalleryImageViewModel>();

        //HIDDEN inputs
        public bool KeepCoverImage { get; set; } = true;
        public bool KeepFeaturedImage { get; set; } = true;

        // New Product Data Fields
        public string ProductType { get; set; } = null!;

        // Pricing Tab
        public decimal? RegularPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public DateTime? SaleStartDate { get; set; }
        public DateTime? SaleEndDate { get; set; }

        // Inventory Tab
        public string? SKU { get; set; }
        public string? GTIN { get; set; }

        public bool ManageStock { get; set; } = false; // Checkbox for stock management

        public string? StockStatus { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be a non-negative value.")]
        public int? Quantity { get; set; }

        public BackorderStatus? AllowBackorders { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Low stock threshold must be a non-negative value.")]
        public int? LowStockThreshold { get; set; }

        public bool SoldIndividually { get; set; } = false; // Limits purchase to 1 per order
        public bool ExcludeFromSoldOutBadge { get; set; } = false; // Exclude from sold-out badge

        public bool IsPublished { get; set; } = true; // Checkbox for product status

        // used in linked products tab {
        public List<int> LinkedProductIds { get; set; } = new List<int>();

        public IEnumerable<SelectListItem> AvailableProducts { get; set; } = new List<SelectListItem>(); // All products

        public IEnumerable<SelectListItem> FilteredProducts { get; set; } = new List<SelectListItem>(); // Products filtered by category
                                                                                                        // }

        // used in product attributes {
        public List<ProductAttributeViewModel> AvailableAttributes { get; set; } = new();

        // Selected attributes (linked to variants)
        public List<ProductAttributeMappingViewModel> SelectedAttributes { get; set; } = new List<ProductAttributeMappingViewModel>();
        // }

        // product Tabs
        public bool DescriptionTab { get; set; }
        public bool ReviewTab { get; set; }
        public List<ProductTabViewModel>? ProductTabs { get; set; } = new();
    }
}
