using Microsoft.AspNetCore.Mvc.Rendering;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class PermissionsFormViewModel
    {
        public string RoleId { get; set; } = null!;
        public string? RoleName { get; set; }
        public List<CheckBoxViewModel> RoleClaims { get; set; } = new List<CheckBoxViewModel>();

        // For Row Level Permissions
        public IEnumerable<SelectListItem>? Categories { get; set; }
        public CategoryPermissionViewModel? CategoriesPermissions { get; set; }
        public CategoryPermissionViewModel? PagesPermissions { get; set; }
    }
}
