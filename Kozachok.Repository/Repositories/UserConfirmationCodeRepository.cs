using Kozachok.Repository.Contexts;
using Kozachok.Repository.Repositories.Common;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Models.DbEntities;

namespace Kozachok.Repository.Repositories
{
    public class UserConfirmationCodeRepository : CrudRepository<UserConfirmationCode>, IUserConfirmationCodeRepository
    {
        public UserConfirmationCodeRepository(MainDbContext context) : base(context)
        {
        }
    }
}
