using Kozachok.Repository.Config;
using Kozachok.Shared.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Kozachok.Repository.Contexts
{
    public class EventStoreSQLContext : DbContext
    {
        public EventStoreSQLContext(DbContextOptions<EventStoreSQLContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddAssemblyConfiguration<IEventMap>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
