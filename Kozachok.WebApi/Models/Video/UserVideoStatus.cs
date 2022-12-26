namespace Kozachok.WebApi.Models.Video
{
    public class UserVideoStatus
    {
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public int Status { get; set; }
        public string? VideoTimeStamp { get; set; }
        public string? AdditionalData { get; set; }
    }
}
