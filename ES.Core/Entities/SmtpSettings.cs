namespace ES.Core.Entities
{
    public class SmtpSettings
    {
        public int Id { get; set; }
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool EnableSsl { get; set; }
    }
}
