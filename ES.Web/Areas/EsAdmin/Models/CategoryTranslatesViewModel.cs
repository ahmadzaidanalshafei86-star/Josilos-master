namespace ES.Web.Areas.EsAdmin.Models
{
    public class CategoryTranslatesViewModel
    {
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedDate { get; set; }

        public string? CategoryDefaultLang { get; set; }

        public IEnumerable<CategoryTranslate>? PreEnteredTranslations { get; set; }
    }
}
