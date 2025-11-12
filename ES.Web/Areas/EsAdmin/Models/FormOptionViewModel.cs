namespace ES.Web.Areas.EsAdmin.Models
{
    public class FormOptionViewModel
    {
        public int? Id { get; set; }
        public int Order { get; set; }
        public string OptionText { get; set; } = null!;
    }
}
