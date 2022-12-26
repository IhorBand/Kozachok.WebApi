using Kozachok.Repository.Config;
using Kozachok.Shared.Abstractions.Mapping;
using Kozachok.Shared.DTO.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kozachok.Repository.Mapping
{
    internal class StoredEventMap : DbEntityConfiguration<StoredEvent>, IEventMap
    {
        public override void Configure(EntityTypeBuilder<StoredEvent> entity)
        {
            entity.ToTable("T_Stored_Event", "dbo");
            entity.Property(p => p.Id).HasColumnName("Id");
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
        }
    }
}
