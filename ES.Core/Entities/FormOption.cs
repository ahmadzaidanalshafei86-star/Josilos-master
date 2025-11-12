namespace ES.Core.Entities
{
    public class FormOption
    {
        public int Id { get; set; }
        public string OptionText { get; set; } = null!;
        public int Order { get; set; }

        //Realtions
        public int FieldId { get; set; }
        public FormField Field { get; set; } = null!;

        public ICollection<OptionTranslation> Translations { get; set; } = new List<OptionTranslation>();
    }
}
