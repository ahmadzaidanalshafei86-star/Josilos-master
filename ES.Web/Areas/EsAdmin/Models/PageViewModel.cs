namespace ES.Web.Areas.EsAdmin.Models
{
    public class PageViewModel
    {
        public int Id { get; set; }
        public string? Slug { get; set; }
        public string Title { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public int CategoryId { get; set; }
        public int Order { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsPublished { get; set; }
    }
}
