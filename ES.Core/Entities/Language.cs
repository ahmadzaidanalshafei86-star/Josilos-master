using System.ComponentModel.DataAnnotations;


namespace ES.Core.Entities
{
    public class Language
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; } = null!;
        [MaxLength(10)]
        public string Code { get; set; } = null!;
    }
}
