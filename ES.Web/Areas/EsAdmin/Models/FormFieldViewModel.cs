namespace ES.Web.Areas.EsAdmin.Models
{
    public class FormFieldViewModel
    {
        public int? Id { get; set; } // Nullable for new fields
        public string FieldName { get; set; } = null!;
        public string? FieldHint { get; set; }
        public string FieldType { get; set; } = null!;
        public bool IsRequired { get; set; }
        public bool IsPublished { get; set; }
        public int Order { get; set; }
        public List<FormOptionViewModel> Options { get; set; } = new();
    }
}
