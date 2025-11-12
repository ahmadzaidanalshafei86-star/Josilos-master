using System.ComponentModel.DataAnnotations;

namespace ES.Web.Areas.EsAdmin.Models
{
    public class SmtpSettingsViewModel
    {
        [Required(ErrorMessage = Errors.RequiredField)]
        public string Host { get; set; } = null!;
        [Required(ErrorMessage = Errors.RequiredField)]
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string? Password { get; set; }
        public bool EnableSsl { get; set; }
        public int Port { get; set; }
    }
}
