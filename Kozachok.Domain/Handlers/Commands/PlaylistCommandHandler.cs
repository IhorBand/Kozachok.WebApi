using Kozachok.Domain.Handlers.Common;
using Kozachok.Shared.Abstractions.Bus;
using Kozachok.Shared.Abstractions.Identity;
using Kozachok.Shared.Abstractions.Repositories.Common;
using Kozachok.Shared.Abstractions.Repositories;
using Kozachok.Shared.DTO.Common;
using Kozachok.Shared.DTO.Models.DbEntities;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;
using Kozachok.Domain.Commands.Playlist;
using Kozachok.Utils.Validation;
using System.Linq;
using Kozachok.Domain.Queries.MovieCatalog;

namespace Kozachok.Domain.Handlers.Commands
{
    public class PlaylistCommandHandler :
        CommandHandler,
        IRequestHandler<AddMovieToPlaylistCommand>,
        IRequestHandler<DeleteMovieFromPlaylistCommand>
    {
        private readonly IUserRepository userRepository;
        private readonly IRoomRepository roomRepository;
        private readonly IRoomUserRepository roomUserRepository;
        private readonly IPlaylistMovieRepository playlistMovieRepository;
        private readonly IPlaylistQualityRepository playlistQualityRepository;
        private readonly IMovieRepository movieRepository;
        private readonly ITranslatorRepository translatorRepository;
        private readonly IUser user;

        public PlaylistCommandHandler(
            IUnitOfWork uow,
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IUserRepository userRepository,
            IRoomRepository roomRepository,
            IRoomUserRepository roomUserRepository,
            IPlaylistMovieRepository playlistMovieRepository,
            IPlaylistQualityRepository playlistQualityRepository,
            IMovieRepository movieRepository,
            ITranslatorRepository translatorRepository,
            IUser user
        )
        : base(
                uow,
                bus,
                notifications
        )
        {
            this.userRepository = userRepository;
            this.roomRepository = roomRepository;
            this.roomUserRepository = roomUserRepository;
            this.playlistMovieRepository = playlistMovieRepository;
            this.playlistQualityRepository = playlistQualityRepository;
            this.movieRepository = movieRepository;
            this.translatorRepository = translatorRepository;
            this.user = user;
        }

        public async Task<Unit> Handle(AddMovieToPlaylistCommand request, CancellationToken cancellationToken)
        {
            if (IsUserAuthorized(user) == false)
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            request
                .IsInvalidGuid(e => e.RoomId, async () => await bus.InvokeDomainNotificationAsync("RoomId is invalid."))
                .IsInvalidGuid(e => e.MovieId, async () => await bus.InvokeDomainNotificationAsync("MovieId is invalid."));

            var currentUser = await userRepository.GetAsync(user.Id.Value);

            if (currentUser == null || (currentUser != null && currentUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var room = await roomRepository.GetAsync(request.RoomId);

            if (room == null || (room != null && room.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("Room doesn't exist.");
                return Unit.Value;
            }

            var roomUser = await roomUserRepository.FirstOrDefaultAsync(ru => ru.UserId == user.Id && ru.RoomId == request.RoomId);

            if (roomUser == null || (roomUser != null && roomUser.Id == Guid.Empty) || (roomUser != null && !roomUser.IsOwner))
            {
                await bus.InvokeDomainNotificationAsync("User doesn't have permission to use this room.");
                return Unit.Value;
            }

            var movie = await movieRepository.GetAsync(request.MovieId);

            if (movie == null || (movie != null && movie.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("Movie doesn't exist.");
                return Unit.Value;
            }

            var translator = await translatorRepository.FirstOrDefaultAsync(t => t.TranslatorId == request.TranslatorId);

            var translatorName = string.Empty;

            if (translator != null)
            {
                translatorName = translator.Name;
            }

            var movieName = movie.ShortTitle;
            if (!string.IsNullOrEmpty(translatorName))
            {
                movieName += $" | {translatorName}";
            }

            movieName += $" | {request.Season}s | {request.Episode}e";

            var playlistMovieCount = playlistMovieRepository.Query().Where(m => m.RoomId == request.RoomId).Count();

            var orderNumber = playlistMovieCount > 0 ? playlistMovieRepository.Query().Where(m => m.RoomId == request.RoomId).Max(m => m.OrderNumber) : 0;

            var movieStream = await bus.RequestAsync(new GetMovieStreamQuery()
            {
                MovieId = request.MovieId,
                TranslatorId = request.TranslatorId,
                Season = request.Season,
                Episode = request.Episode
            });

            if (movieStream == null || (movieStream != null && movieStream.resolutions == null))
            {
                await bus.InvokeDomainNotificationAsync("Cannot Fetch Movie Stream.");
                return Unit.Value;
            }

            var playlistMovie = new PlaylistMovie()
            {
                MovieId = request.MovieId,
                RoomId = request.RoomId,
                Name = movieName,
                OrderNumber = ++orderNumber,
                VideoId = movie.VideoId,
                VideoUrlId = movie.VideoUrlId
            };
            await playlistMovieRepository.AddAsync(playlistMovie);

            bool isStreamFetched = false;

            foreach (var resolution in movieStream.resolutions)
            {
                if (string.IsNullOrEmpty(resolution.link))
                {
                    continue;
                }
                else
                {
                    isStreamFetched = true;
                }

                var resolutionDefinition = Shared.DTO.Enums.VideoQualityType.Low_360;
                switch (resolution.resolutionName)
                {
                    case "360p":
                        resolutionDefinition = Shared.DTO.Enums.VideoQualityType.Low_360;
                        break;
                    case "480p":
                        resolutionDefinition = Shared.DTO.Enums.VideoQualityType.Medium_480;
                        break;
                    case "720p":
                        resolutionDefinition = Shared.DTO.Enums.VideoQualityType.High_720;
                        break;
                    case "1080p":
                        resolutionDefinition = Shared.DTO.Enums.VideoQualityType.HD_1080;
                        break;
                    case "1080p Ultra":
                        resolutionDefinition = Shared.DTO.Enums.VideoQualityType.UltraHD_1080;
                        break;
                }
                var playlistQuality = new PlaylistQuality()
                {
                    MovieUrl = resolution.link,
                    PlaylistMovieId = playlistMovie.Id,
                    QualityId = resolutionDefinition
                };
                await playlistQualityRepository.AddAsync(playlistQuality);
            }

            if (!isStreamFetched)
            {
                await bus.InvokeDomainNotificationAsync("Cannot Fetch Movie Stream.");
                return Unit.Value;
            }

            Commit();

            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteMovieFromPlaylistCommand request, CancellationToken cancellationToken)
        {
            if (IsUserAuthorized(user) == false)
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            request
                .IsInvalidGuid(e => e.PlaylistMovieId, async () => await bus.InvokeDomainNotificationAsync("RoomId is invalid."));

            var currentUser = await userRepository.GetAsync(user.Id.Value);

            if (currentUser == null || (currentUser != null && currentUser.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var playlistMovie = await playlistMovieRepository.GetAsync(request.PlaylistMovieId);

            if (playlistMovie == null || (playlistMovie != null && playlistMovie.Id == Guid.Empty))
            {
                await bus.InvokeDomainNotificationAsync("Playlist for Room doesn't exist.");
                return Unit.Value;
            }

            var roomUser = await roomUserRepository.FirstOrDefaultAsync(ru => ru.UserId == user.Id && ru.RoomId == playlistMovie.RoomId);

            if (roomUser == null || (roomUser != null && roomUser.Id == Guid.Empty) || (roomUser != null && !roomUser.IsOwner))
            {
                await bus.InvokeDomainNotificationAsync("User doesn't have permission to use this room.");
                return Unit.Value;
            }

            var playlistQualities = playlistQualityRepository.Query().Where(q => q.PlaylistMovieId == playlistMovie.Id).ToList();

            foreach (var playlistQuality in playlistQualities)
            {
                await playlistQualityRepository.DeleteAsync(playlistQuality.Id);
            }

            Commit();

            await playlistMovieRepository.DeleteAsync(playlistMovie.Id);

            Commit();

            return Unit.Value;
        }
    }
}
