using Microsoft.EntityFrameworkCore;

namespace Kozachok.Repository.Config
{
    public interface IDbEntityConfiguration
    {
        void Configure(ModelBuilder modelBuilder);
    }
}
