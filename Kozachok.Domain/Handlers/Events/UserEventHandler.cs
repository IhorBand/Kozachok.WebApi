using Kozachok.Domain.Events.User;
using MediatR;
using System.Threading.Tasks;
using System.Threading;

namespace Kozachok.Domain.Handlers.Events
{
    public class UserEventHandler : INotificationHandler<CreateUserEvent>, INotificationHandler<UpdateUserEvent>, INotificationHandler<DeleteUserEvent>, INotificationHandler<ChangeUserPasswordEvent>
    {
        public Task Handle(CreateUserEvent notification, CancellationToken cancellationToken)
        {
            // TODO: send confirmation email
            return Task.CompletedTask;
        }

        public Task Handle(UpdateUserEvent notification, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task Handle(DeleteUserEvent notification, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task Handle(ChangeUserPasswordEvent notification, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
