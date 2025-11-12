namespace ES.Web.Areas.EsAdmin.Models
{
    public class EcomCategoryViewModel
    {
        public int Id { get; set; }
        public string? Slug { get; set; }
        public string Name { get; set; } = null!;
        public string? FeaturedImageUrl { get; set; }
        public string? FeaturedImageAltName { get; set; }
        public string? ParentCategoryName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsPublished { get; set; }
    }
}
