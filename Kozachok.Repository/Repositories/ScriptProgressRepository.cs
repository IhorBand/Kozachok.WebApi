using Kozachok.Repository.Contexts;
using Kozachok.Repository.Repositories.Common;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Repository.Repositories
{
    public class ScriptProgressRepository : CrudRepository<ScriptProgress>, IScriptProgressRepository
    {
        public ScriptProgressRepository(MainDbContext context) : base(context)
        {
        }
    }
}
