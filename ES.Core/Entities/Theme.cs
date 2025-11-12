using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace ES.Core.Entities
{
    [Index(nameof(ThemeName), IsUnique = true)]
    public class Theme
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string ThemeName { get; set; } = null!;
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
