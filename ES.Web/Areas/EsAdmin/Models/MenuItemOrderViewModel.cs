namespace ES.Web.Areas.EsAdmin.Models
{
    public class MenuItemOrderViewModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int Order { get; set; }
    }
}
