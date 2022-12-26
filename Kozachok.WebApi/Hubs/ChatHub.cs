using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Kozachok.WebApi.Hubs.Base;
using SignalRSwaggerGen.Attributes;
using Kozachok.WebApi.Models.Chat;

namespace Kozachok.WebApi.Hubs
{
    [SignalRHub]
    [Authorize]
    public class ChatHub : UserHubBase
    {
        private readonly ILogger<ChatHub> logger;
        public ChatHub(
            ILogger<ChatHub> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
    }
}
