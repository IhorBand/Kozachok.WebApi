using Kozachok.Repository.Config;
using Kozachok.Shared.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kozachok.Repository.Contexts
{
    public class MainDbContext : DbContext
    {
        private readonly ILoggerFactory loggerFactory;

        public MainDbContext(DbContextOptions<MainDbContext> options, ILoggerFactory loggerFactory) : base(options)
        {
            this.loggerFactory = loggerFactory;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddAssemblyConfiguration<IEntityMap>();
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(loggerFactory);
        }
    }
}
