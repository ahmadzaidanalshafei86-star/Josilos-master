using System.ComponentModel.DataAnnotations.Schema;

namespace ES.Core.Entities
{
    public class FormField
    {
        public int Id { get; set; }
        public string FieldName { get; set; } = null!;
        public string? FieldHint { get; set; }
        public string FieldType { get; set; } = null!;
        public bool IsRequired { get; set; }
        public bool IsPublished { get; set; }
        public int Order { get; set; }


        //Realtions
        public int FormId { get; set; }
        public Form Form { get; set; } = null!;
        public ICollection<FormOption> Options { get; set; } = new List<FormOption>();
        public ICollection<FormResponseDetail> ResponseDetails { get; set; } = new List<FormResponseDetail>();
        public ICollection<FieldTranslation> Translations { get; set; } = new List<FieldTranslation>();

        [NotMapped]
        public string? DisplayName { get; set; }
    }
}
