using System;

namespace Kozachok.Shared.Abstractions.Identity
{
    public interface IUser
    {
        Guid? Id { get; }
        string Name { get; }
        string Email { get; }
    }
}
