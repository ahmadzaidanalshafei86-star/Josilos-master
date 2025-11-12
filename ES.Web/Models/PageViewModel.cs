namespace ES.Web.Models
{
    public class PageViewModel
    {
        public string? Slug { get; set; }
        public string? Title { get; set; }
        public string? LongDescription { get; set; }
        public string? ShortDescription { get; set; }
        public string? FeaturedImageUrl { get; set; }
        public string? FeaturedImageAltName { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? CoverImageAltName { get; set; }
        public string? VideoUrl { get; set; }
        public int Order { get; set; }
        public string? ContactNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? DateInput { get; set; }

        public IList<GalleryImageViewModel>? GalleryImages = new List<GalleryImageViewModel>();
        public string? GalleryStyle { get; set; }

        public int Count { get; set; }

        public Form PageForm { get; set; } = new Form();
        public bool HaveForm { get; set; } = false;
        public string? LinkUrl { get; set; }
    }
}
