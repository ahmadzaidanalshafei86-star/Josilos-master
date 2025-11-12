using System.ComponentModel.DataAnnotations;

namespace ES.Core.Entities
{
    public class ProductGalleryImage
    {
        public int Id { get; set; }
        [MaxLength(255)]
        public string AltName { get; set; } = null!;
        [MaxLength(255)]
        public string GalleryImageUrl { get; set; } = null!;
        public int DisplayOrder { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
