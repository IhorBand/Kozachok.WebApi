using Kozachok.Shared.Abstractions.Commands;
using Kozachok.Shared.Abstractions.Events;
using System.Threading.Tasks;

namespace Kozachok.Shared.Abstractions.Bus
{
    public interface IMediatorHandler
    {
        Task SendAsync<T>(T command) where T : Command;
        Task<TResult> RequestAsync<TResult>(RequestCommand<TResult> command);
        Task InvokeAsync<T>(T @event) where T : Event;
        Task InvokeDomainNotificationAsync(string message);
    }
}
