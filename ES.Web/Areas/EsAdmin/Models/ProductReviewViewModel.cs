namespace ES.Web.Areas.EsAdmin.Models
{
    public class ProductReviewViewModel
    {
        public int ReviewId { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string ProductName { get; set; } = null!;
        public bool IsPublished { get; set; }
        public List<ApplicationDetailViewModel> Details { get; set; } = new List<ApplicationDetailViewModel>();
    }
}
