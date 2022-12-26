using Kozachok.Repository.Config;
using Kozachok.Shared.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;

namespace Kozachok.Repository.Contexts
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddAssemblyConfiguration<IEntityMap>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
