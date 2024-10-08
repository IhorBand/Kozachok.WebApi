﻿using Kozachok.Repository.Config;
using Kozachok.Shared.Abstractions.Mapping;
using Kozachok.Shared.DTO.Models.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kozachok.Repository.Mapping
{
    internal class RoomMap : DbEntityConfiguration<Room>, IEntityMap
    {
        public override void Configure(EntityTypeBuilder<Room> entity)
        {
            entity.ToTable("T_Room", "dbo");
            entity.Property(p => p.Id).HasColumnName("Id");
            entity.Property(p => p.Id).ValueGeneratedOnAdd();


            entity.HasMany(x => x.RoomUsers)
                .WithOne(x => x.Room);
            
            entity.HasMany(x => x.PlaylistMovies)
                .WithOne(x => x.Room);
        }
    }
}
