﻿using Kozachok.Repository.Config;
using Kozachok.Shared.Abstractions.Mapping;
using Kozachok.Shared.DTO.Models.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Kozachok.Repository.Mapping
{
    internal class UserMap : DbEntityConfiguration<User>, IEntityMap
    {
        public override void Configure(EntityTypeBuilder<User> entity)
        {
            entity.ToTable("T_User", "dbo");
            entity.Property(c => c.Id).HasColumnName("Id");
            entity.Property(p => p.Id).ValueGeneratedOnAdd();


            entity.HasMany(x => x.RoomUsers)
                .WithOne(x => x.User);

            entity.HasOne(x => x.ThumbnailImageFile)
                .WithMany(x => x.Users);
        }
    }
}
