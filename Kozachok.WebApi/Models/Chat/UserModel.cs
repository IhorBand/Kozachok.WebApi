namespace Kozachok.WebApi.Models.Chat
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
