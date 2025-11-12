using System.ComponentModel.DataAnnotations;

namespace ES.Core.Entities
{
    public class Notification
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string WhatsAppNumber { get; set; } = string.Empty;
    }

}
