namespace ES.Web.Areas.EsAdmin.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string? Slug { get; set; }
        public string Title { get; set; } = null!;
        public string? FeaturedImageUrl { get; set; }
        public string? FeaturedImageAltName { get; set; }
        public List<string> CategoryNames { get; set; } = new();

        public bool IsPublished { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
