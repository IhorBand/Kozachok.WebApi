using Kozachok.Repository.Contexts;
using Kozachok.Repository.Repositories.Common;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Repository.Repositories
{
    public class FileRepository : CrudRepository<File>, IFileRepository
    {
        public FileRepository(MainDbContext context) : base(context)
        {
        }
    }
}
