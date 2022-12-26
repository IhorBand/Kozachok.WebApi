using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Kozachok.WebApi.Hubs.Base;
using Kozachok.WebApi.Models.Chat;
using Kozachok.WebApi.Models.Video;
using SignalRSwaggerGen.Attributes;

namespace Kozachok.WebApi.Hubs
{
    [SignalRHub]
    [Authorize]
    public class VideoHub : UserHubBase
    {
        private readonly ILogger<VideoHub> logger;
        public VideoHub(
            ILogger<VideoHub> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [SignalRHidden]
        public async override Task OnConnectedAsync()
        {
            var user = new UserModel()
            {
                Id = this.UserId,
                UserName = this.UserName,
                AvatarUrl = ""
            };

            await this.Clients.AllExcept(this.Context.ConnectionId).SendAsync("NewWatcherConnected", user).ConfigureAwait(false);

            await base.OnConnectedAsync();
        }

        [SignalRHidden]
        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            var user = new UserModel()
            {
                Id = this.UserId,
                UserName = this.UserName,
                AvatarUrl = ""
            };

            await this.Clients.AllExcept(this.Context.ConnectionId).SendAsync("WatcherDisconnected", user).ConfigureAwait(false);

            await base.OnDisconnectedAsync(exception);
        }

        //public async Task SendVideoAsync(string name)
        //{
        //    var video = await this.videoPlaylistService.InsertVideoToPlaylistAsync(new ModelsDB.VideoPlaylist()
        //    {
        //        CreatedDateUtc = DateTime.UtcNow,
        //        Id = Guid.NewGuid(),
        //        Name = name
        //    });

        //    await this.Clients.All.SendAsync("ReceiveVideo", 
        //        new VideoModel() { 
        //            Id = video.Id,
        //            Name = name
        //        }).ConfigureAwait(false);
        //}

        //public async Task SendVideoQualityAsync(string videoId, string url, string name)
        //{
        //    var videoQuallity = await this.videoQualityService.InsertVideoQualityAsync(new ModelsDB.VideoQuality()
        //    {
        //        Id = Guid.NewGuid(),
        //        Name = name,
        //        Url = url,
        //        VideoId = new Guid(videoId),
        //        CreatedDateUtc = DateTime.UtcNow
        //    });

        //    var model = new VideoQualityModel()
        //    {
        //        Id = videoQuallity.Id,
        //        Name = name,
        //        Url = url,
        //        VideoId = new Guid(videoId)
        //    };

        //    await this.Clients.All.SendAsync("ReceiveVideoQuality", model).ConfigureAwait(false);
        //}

        public async Task SendNewVideoTimeStampAsync(string videoId, string timestamp, bool isForce)
        {
            var model = new VideoTimeStampModel()
            {
                VideoId = new Guid(videoId),
                TimeStamp = timestamp,
                IsForce = isForce
            };

            await this.Clients.All.SendAsync("ReceiveNewVideoTimeStamp", model).ConfigureAwait(false);
        }

        public async Task SendСhangeVideoAsync(string videoId, string timestamp)
        {
            var model = new VideoTimeStampModel()
            {
                VideoId = new Guid(videoId),
                TimeStamp = timestamp,
                IsForce = true
            };

            await this.Clients.All.SendAsync("ReceiveChangeVideo", model).ConfigureAwait(false);
        }

        public async Task SendStartVideoAsync(string videoId, string timestamp)
        {
            var model = new VideoTimeStampModel()
            {
                VideoId = new Guid(videoId),
                TimeStamp = timestamp,
                IsForce = true
            };

            await this.Clients.All.SendAsync("ReceiveStartVideo", model).ConfigureAwait(false);
        }

        public async Task SendStopVideoAsync(string videoId, string timestamp)
        {
            var model = new VideoTimeStampModel()
            {
                VideoId = new Guid(videoId),
                TimeStamp = timestamp,
                IsForce = true
            };

            await this.Clients.All.SendAsync("ReceiveStopVideo", model).ConfigureAwait(false);
        }

        public async Task SendUserStatusAsync(int userStatus, string videoTimeStamp, string additionalData)
        {
            await this.Clients.AllExcept(this.Context.ConnectionId).SendAsync("ReceiveUserStatus", new UserVideoStatus() { 
                Status = userStatus,
                UserId = this.UserId,
                UserName = this.UserName,
                VideoTimeStamp = videoTimeStamp,
                AdditionalData = additionalData
            }).ConfigureAwait(false);
        }
    }
}
