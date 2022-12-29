using Kozachok.Repository.Contexts;
using Kozachok.Repository.Repositories.Common;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Repository.Repositories
{
    public class UserConfirmationCodeRepository : CrudRepository<UserConfirmationCode>, IUserConfirmationCodeRepository
    {
        public UserConfirmationCodeRepository(MainDbContext context) : base(context)
        {
        }
    }
}
