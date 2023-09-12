using Kozachok.Repository.Contexts;
using Kozachok.Repository.Repositories.Common;
using Kozachok.Shared.Abstractions.Events;
using Kozachok.Shared.Abstractions.Identity;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Models.DbEntities;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Kozachok.Repository.Repositories
{
    public class StoredEventRepository : CrudRepository<StoredEvent>, IStoredEventRepository
    {
        private readonly IUser user;

        public StoredEventRepository(EventStoreSqlContext context, IUser user) : base(context) => this.user = user;

        public async Task AddEventAsync<TEvent>(TEvent @event) where TEvent : Event
        {
            await AddAsync(new StoredEvent(@event, JsonConvert.SerializeObject(@event), user?.Name));
            await Context.SaveChangesAsync();
        }
    }
}
