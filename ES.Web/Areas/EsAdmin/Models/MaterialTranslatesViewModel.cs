

namespace ES.Web.Areas.EsAdmin.Models
{
    public class MaterialTranslatesViewModel
    {
        public int? MaterialId { get; set; }
        public string? MaterialName { get; set; }
        public DateTime CreatedDate { get; set; }

        public string? MaterialDefaultLang { get; set; }

        public IEnumerable<MaterialTranslate>? PreEnteredTranslations { get; set; }
    }
}
