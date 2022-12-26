﻿using Kozachok.Repository.Contexts;
using Kozachok.Repository.Repositories.Common;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Models;

namespace Kozachok.Repository.Repositories
{
    public class UserRepository : CrudRepository<User>, IUserRepository
    {
        public UserRepository(MainDbContext context) : base(context)
        {
        }
    }
}
