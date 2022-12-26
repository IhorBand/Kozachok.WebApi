namespace Kozachok.WebApi.Models.Video
{
    public class VideoQualityModel
    {
        public Guid Id { get; set; }
        public Guid VideoId { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
    }
}
