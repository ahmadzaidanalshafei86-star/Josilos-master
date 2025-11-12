namespace ES.Web.Areas.EsAdmin.Models
{
    public class CareerTranslateViewModel
    {
        public int? CareerId { get; set; }
        public string? CareerName { get; set; }
        public DateTime CreatedDate { get; set; }

        public string? CareerDefaultLang { get; set; }

        public IEnumerable<CareerTranslate>? PreEnteredTranslations { get; set; }
    }
}
