namespace ES.Web.Areas.EsAdmin.Models
{
    public class FileViewModel
    {
        public string FileUrl { get; set; } = null!;
        public string FileAltName { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }
}
