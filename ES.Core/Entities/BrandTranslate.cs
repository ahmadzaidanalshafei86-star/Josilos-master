namespace ES.Core.Entities
{
    public class BrandTranslate
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;
        public int? LanguageId { get; set; }
        public Language? Language { get; set; }
    }
}
