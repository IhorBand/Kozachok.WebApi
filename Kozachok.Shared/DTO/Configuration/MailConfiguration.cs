namespace Kozachok.Shared.DTO.Configuration
{
    public class MailConfiguration
    {
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ApiKey { get; set; }
    }
}
