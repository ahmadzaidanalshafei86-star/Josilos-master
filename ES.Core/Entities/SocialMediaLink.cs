using System.ComponentModel.DataAnnotations;

namespace ES.Core.Entities
{
    public class SocialMediaLink
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string? Name { get; set; }
        [MaxLength(200)]
        public string Url { get; set; } = string.Empty;
        public string? IconClass { get; set; }
        public string? IconColor { get; set; }
        public bool IsPublished { get; set; } = true;
    }
}
