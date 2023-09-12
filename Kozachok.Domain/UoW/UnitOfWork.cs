using Kozachok.Repository.Contexts;
using Kozachok.Shared.Abstractions.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Kozachok.Domain.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext context;

        public UnitOfWork(MainDbContext context) => this.context = context;

        public void Commit()
        {
            if (context.ChangeTracker.Entries().Any(e => new[] { EntityState.Added, EntityState.Deleted, EntityState.Modified }.Contains(e.State)))
                context.SaveChanges();
        }

        public void Dispose() => context.Dispose();
    }
}
