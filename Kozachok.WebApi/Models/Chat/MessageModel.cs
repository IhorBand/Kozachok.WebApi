namespace Kozachok.WebApi.Models.Chat
{
    public class MessageModel
    {
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? Message { get; set; }
    }
}
