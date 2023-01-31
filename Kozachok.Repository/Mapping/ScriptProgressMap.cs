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
    internal class ScriptProgressMap : DbEntityConfiguration<ScriptProgress>, IEntityMap
    {
        public override void Configure(EntityTypeBuilder<ScriptProgress> entity)
        {
            entity.ToTable("T_Script_Progress", "dbo");
            entity.Property(c => c.Id).HasColumnName("Id");
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
        }
    }
}
