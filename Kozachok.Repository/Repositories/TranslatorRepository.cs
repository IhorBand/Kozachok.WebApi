using Kozachok.Repository.Contexts;
using Kozachok.Repository.Repositories.Common;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Models.DbEntities;

namespace Kozachok.Repository.Repositories
{
    public class TranslatorRepository : CrudRepository<Translator>, ITranslatorRepository
    {
        public TranslatorRepository(MainDbContext context) : base(context)
        {
        }
    }
}
