namespace ES.Core.Entities
{
    public class FormResponseDetail
    {
        public int Id { get; set; }
        public string? ResponseValue { get; set; }
        public int ResponseId { get; set; }
        public FormResponse Response { get; set; } = null!;
        public int FieldId { get; set; }
        public FormField Field { get; set; } = null!;
    }
}
