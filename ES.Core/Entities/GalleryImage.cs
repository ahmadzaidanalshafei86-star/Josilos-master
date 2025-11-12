using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ES.Core.Entities
{
    public class GalleryImage
    {
        public int Id { get; set; }
        [MaxLength(255)]
        public string AltName { get; set; } = null!;
        [MaxLength(255)]
        public string GalleryImageUrl { get; set; } = null!;
        public int DisplayOrder { get; set; }
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
