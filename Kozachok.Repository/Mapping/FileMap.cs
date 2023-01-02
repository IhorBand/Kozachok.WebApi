using Kozachok.Repository.Config;
using Kozachok.Shared.Abstractions.Mapping;
using Kozachok.Shared.DTO.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kozachok.Repository.Mapping
{
    internal class FileMap : DbEntityConfiguration<File>, IEntityMap
    {
        public override void Configure(EntityTypeBuilder<File> entity)
        {
            entity.ToTable("T_File", "dbo");
            entity.Property(p => p.Id).HasColumnName("Id");
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
        }
    }
}
