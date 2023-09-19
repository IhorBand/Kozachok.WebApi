using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Kozachok.WebApi.Hubs.Base;
using SignalRSwaggerGen.Attributes;
using Kozachok.WebApi.Models.Chat;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Domain.Commands.ChatConnection;
using Kozachok.Shared.DTO.Common;
using MediatR;
using Kozachok.Domain.Handlers.Notifications;
using Kozachok.WebApi.Models.Common;

namespace Kozachok.WebApi.Hubs
{
    [SignalRHub]
    [Authorize]
    public class ChatHub : UserHubBase
    {
        private readonly ILogger<ChatHub> logger;
        private readonly IMediatorHandler bus;
        private readonly DomainNotificationHandler notifications;

        public ChatHub(
            ILogger<ChatHub> logger,
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.bus = bus;
            this.notifications = (DomainNotificationHandler)notifications;
        }

        [SignalRHidden]
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        [SignalRHidden]
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRoomChat(Guid roomId)
        {
            await bus.SendAsync(new JoinRoomChatCommand()
            {
                UserId = UserId,
                RoomId = roomId,
                ConnectionId = Context.ConnectionId
            });

            if (!IsValidOperation())
            {
                await Clients.Caller.SendAsync("ErrorHandler", "something went wrong while trying to join to room.");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        }

        public async Task SendMessageAsync(string message)
        {
            var messageModel = new MessageModel()
            {
                Message = message,
                UserId = this.UserId,
                UserName = this.UserName
            };

            await this.Clients.All.SendAsync("ReceiveMessage", messageModel).ConfigureAwait(false);
        }

        public async Task SendChangeAvatarAsync(int avatarId)
        {
            await this.Clients.All.SendAsync("ReceiveChangeAvatarAsync", new { AvatarId = avatarId, UserId = this.UserId, UserName = this.UserName }).ConfigureAwait(false);
        }

        protected bool IsValidOperation() => !notifications.HasNotifications();
    }
}
