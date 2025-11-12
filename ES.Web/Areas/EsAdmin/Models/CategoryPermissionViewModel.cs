namespace ES.Web.Areas.EsAdmin.Models
{
    public class CategoryPermissionViewModel
    {
        public List<int>? CategoryIds { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }
}
