namespace Kozachok.Shared.DTO.Models.Result.User
{
    public class UserDetails
    {
        public DbEntities.User User { get; set; }
        public string ThumbnailImageUrl { get; set; }
    }
}
