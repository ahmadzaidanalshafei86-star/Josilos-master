namespace ES.Web.Areas.EsAdmin.Models
{
    public class CareerApplicationViewModel
    {
        public int ApplicationId { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string CareerName { get; set; } = null!;
        public bool IsReviewed { get; set; }
        public bool IsArchived { get; set; }
        public List<ApplicationDetailViewModel> Details { get; set; } = new List<ApplicationDetailViewModel>();

    }
}
