using Kozachok.Shared.Abstractions.Events;
using Kozachok.Shared.Abstractions.Repositories.Common;
using Kozachok.Shared.DTO.Models;
using System.Threading.Tasks;

namespace Kozachok.Shared.Abstractions.Repositories
{
    public interface IStoredEventRepository : ICrudRepository<StoredEvent>
    {
        Task AddEventAsync<TEvent>(TEvent @event) where TEvent : Event;
    }
}
