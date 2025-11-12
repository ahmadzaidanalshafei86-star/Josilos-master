namespace ES.Web.Areas.EsAdmin.Models
{
    public class EcomCategoryTranslatesViewModel
    {
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedDate { get; set; }

        public string? CategoryDefaultLang { get; set; }

        public IEnumerable<EcomCategoryTranslate>? PreEnteredTranslations { get; set; }
    }
}
