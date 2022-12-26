using System;

namespace Kozachok.Shared.Abstractions.Repositories.Common
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
    }
}
