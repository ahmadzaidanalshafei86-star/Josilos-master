namespace ES.Web.Areas.EsAdmin.Models
{
    public class CareerViewModel
    {
        public int Id { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
