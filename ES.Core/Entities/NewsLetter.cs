using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ES.Core.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    public class NewsLetter
    {
        public int Id { get; set; }

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
