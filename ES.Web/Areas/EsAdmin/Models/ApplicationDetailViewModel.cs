namespace ES.Web.Areas.EsAdmin.Models
{
    public class ApplicationDetailViewModel
    {
        public string FieldName { get; set; } = null!;
        public string FieldType { get; set; } = null!;
        public string? ResponseValue { get; set; }
        public string? FileUrl { get; set; }
    }
}
