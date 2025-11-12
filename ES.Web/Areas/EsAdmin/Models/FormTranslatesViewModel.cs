namespace ES.Web.Areas.EsAdmin.Models
{
    public class FormTranslatesViewModel
    {
        public int? FormId { get; set; }
        public string? FormTitle { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? FormDefaultLang { get; set; }
        public IEnumerable<FormTranslation>? PreEnteredTranslations { get; set; }
    }
}
