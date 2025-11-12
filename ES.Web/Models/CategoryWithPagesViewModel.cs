namespace ES.Web.Models
{
    public class CategoryWithPagesViewModel
    {
        public string? Slug { get; set; }
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? CoverimageAltName { get; set; }
        public string? FeaturedImageUrl { get; set; }
        public string? FeaturedimageAltName { get; set; }
        public string? ThemeName { get; set; }

        public IList<PageViewModel> Pages = new List<PageViewModel>();
    }
}
