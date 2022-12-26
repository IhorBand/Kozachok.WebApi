namespace Kozachok.Shared.Abstractions.Identity
{
    public interface IUser
    {
        string Id { get; }
        string Name { get; }
        string Email { get; }
    }
}
