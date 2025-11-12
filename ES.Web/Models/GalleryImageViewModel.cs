namespace ES.Web.Models
{
    public class GalleryImageViewModel
    {
        public string GalleryImageUrl { get; set; } = null!;
        public string GalleryImageAltName { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }
}
