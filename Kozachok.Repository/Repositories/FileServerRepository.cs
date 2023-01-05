using Kozachok.Repository.Contexts;
using Kozachok.Repository.Repositories.Common;
using Kozachok.Shared.Abstractions.Events;
using Kozachok.Shared.Abstractions.Identity;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Models.DbEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kozachok.Repository.Repositories
{
    public class FileServerRepository : CrudRepository<FileServer>, IFileServerRepository
    {
        public FileServerRepository(MainDbContext context) : base(context)
        {
        }
    }
}
