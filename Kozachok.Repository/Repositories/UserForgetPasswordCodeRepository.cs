using Kozachok.Repository.Contexts;
using Kozachok.Repository.Repositories.Common;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Models;

namespace Kozachok.Repository.Repositories
{
    public class UserForgetPasswordCodeRepository : CrudRepository<UserForgetPasswordCode>, IUserForgetPasswordCodeRepository
    {
        public UserForgetPasswordCodeRepository(MainDbContext context) : base(context)
        {
        }
    }
}
