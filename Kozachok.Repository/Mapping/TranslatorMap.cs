using Kozachok.Repository.Config;
using Kozachok.Shared.Abstractions.Mapping;
using Kozachok.Shared.DTO.Models.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Repository.Mapping
{
    internal class TranslatorMap : DbEntityConfiguration<Translator>, IEntityMap
    {
        public override void Configure(EntityTypeBuilder<Translator> entity)
        {
            entity.ToTable("L_Translator", "dbo");
            entity.Property(p => p.Id).HasColumnName("Id");
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
        }
    }
}
