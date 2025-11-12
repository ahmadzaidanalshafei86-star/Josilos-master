using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class MenuItemViewModel
    {
        public int? Id { get; set; }
        public int? ParentId { get; set; }
        [Required(ErrorMessage = Errors.RequiredField)]
        public string Title { get; set; } = null!;
        public string Type { get; set; } = "CustomLink";
        public int Order { get; set; }
        public bool IsPublished { get; set; }
        public string? Target { get; set; }

        public string? Icon { get; set; }

        [MaxLength(450)]
        public string? URL { get; set; }

        public List<MenuItemViewModel> Children { get; set; } = new List<MenuItemViewModel>();

        public IEnumerable<SelectListItem>? Categories { get; set; }
        public IEnumerable<SelectListItem>? Pages { get; set; }
        public IEnumerable<SelectListItem>? ProductCategories { get; set; }

        // Helper Property to Maintain Menu List
        public List<MenuItemViewModel> MenuItems { get; set; } = new List<MenuItemViewModel>();
    }
}
