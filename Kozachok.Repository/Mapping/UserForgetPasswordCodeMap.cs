using Kozachok.Repository.Config;
using Kozachok.Shared.Abstractions.Mapping;
using Kozachok.Shared.DTO.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Kozachok.Repository.Mapping
{
    internal class UserForgetPasswordCodeMap : DbEntityConfiguration<UserForgetPasswordCode>, IEntityMap
    {
        public override void Configure(EntityTypeBuilder<UserForgetPasswordCode> entity)
        {
            entity.ToTable("T_User_Forget_Password_Code", "dbo");
            entity.Property(c => c.Id).HasColumnName("Id");
            entity.Property(p => p.Id).ValueGeneratedOnAdd();
        }
    }
}
