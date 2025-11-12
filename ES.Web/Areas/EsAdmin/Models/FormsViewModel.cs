namespace ES.Web.Areas.EsAdmin.Models
{
    public class FormsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
