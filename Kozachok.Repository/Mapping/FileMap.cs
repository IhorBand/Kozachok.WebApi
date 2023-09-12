using Kozachok.Repository.Config;
using Kozachok.Shared.Abstractions.Mapping;
using Kozachok.Shared.DTO.Models.DbEntities;
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

            entity.HasOne(x => x.FileServer)
                .WithMany(x => x.Files);
            entity.HasMany(x => x.Users)
                .WithOne(x => x.ThumbnailImageFile);

            //.UsingEntity(x => x.ToTable("AnswerAnswerOptions"));
        }
    }
}
