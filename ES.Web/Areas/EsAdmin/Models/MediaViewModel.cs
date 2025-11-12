namespace ES.Web.Areas.EsAdmin.Models
{
    public class MediaViewModel
    {
        public int Id { get; set; }
        public string CategoryOrPageName { get; set; } = null!; // Name of category or page
        public string? ImageUrl { get; set; }  // URL for images or file paths
        public string? AltName { get; set; }   // Editable Alt text
        public string? Type { get; set; }      // Type of media (e.g., "Category Cover", "Page File")
        public DateTime? CreatedDate { get; set; } // Date of media creation
    }


}
