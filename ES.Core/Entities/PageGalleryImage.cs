using System.ComponentModel.DataAnnotations;

namespace ES.Core.Entities
{
    public class PageGalleryImage
    {
        public int Id { get; set; }
        [MaxLength(255)]
        public string AltName { get; set; } = null!;
        [MaxLength(255)]
        public string GalleryImageUrl { get; set; } = null!;
        public int DisplayOrder { get; set; }

        public int PageId { get; set; }
        public Page Page { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
