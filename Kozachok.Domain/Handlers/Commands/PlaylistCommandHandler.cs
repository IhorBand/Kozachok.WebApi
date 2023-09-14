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
using Microsoft.EntityFrameworkCore;

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
        private readonly IPlaylistMovieVideoRepository playlistMovieVideoRepository;
        private readonly IPlaylistMovieVideoQualityRepository playlistMovieVideoQualityRepository;
        private readonly IMovieRepository movieRepository;
        private readonly IUser user;

        public PlaylistCommandHandler(
            IUnitOfWork uow,
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IUserRepository userRepository,
            IRoomRepository roomRepository,
            IRoomUserRepository roomUserRepository,
            IPlaylistMovieRepository playlistMovieRepository,
            IPlaylistMovieVideoRepository playlistMovieVideoRepository,
            IPlaylistMovieVideoQualityRepository playlistMovieVideoQualityRepository,
            IMovieRepository movieRepository,
            IUser user)
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
            this.playlistMovieVideoRepository = playlistMovieVideoRepository;
            this.playlistMovieVideoQualityRepository = playlistMovieVideoQualityRepository;
            this.movieRepository = movieRepository;
            this.user = user;
        }

        public async Task<Unit> Handle(AddMovieToPlaylistCommand request, CancellationToken cancellationToken)
        {
            if (IsUserAuthorized(user) == false)
            {
                await Bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            request
                .IsInvalidGuid(e => e.RoomId, async () => await Bus.InvokeDomainNotificationAsync("RoomId is invalid."))
                .IsInvalidGuid(e => e.MovieId, async () => await Bus.InvokeDomainNotificationAsync("MovieId is invalid."));

            var currentUser = await userRepository.GetAsync(user.Id.Value, cancellationToken);

            if (currentUser == null || currentUser.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var room = await roomRepository.GetAsync(request.RoomId, cancellationToken);

            if (room == null || room.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("Room doesn't exist.");
                return Unit.Value;
            }

            var roomUser = await roomUserRepository.FirstOrDefaultAsync(ru => ru.UserId == user.Id && ru.RoomId == request.RoomId, cancellationToken);

            if (roomUser == null || roomUser.Id == Guid.Empty || !roomUser.IsOwner)
            {
                await Bus.InvokeDomainNotificationAsync("User doesn't have permission to use this room.");
                return Unit.Value;
            }

            var movie = await movieRepository.GetAsync(request.MovieId, cancellationToken);

            if (movie == null || movie.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("Movie doesn't exist.");
                return Unit.Value;
            }
            
            var playlistMovieCount = await playlistMovieRepository.Query().CountAsync(m => m.RoomId == request.RoomId, cancellationToken);

            var orderNumber = playlistMovieCount > 0 ? 
                await playlistMovieRepository.Query()
                    .Where(m => m.RoomId == request.RoomId)
                    .MaxAsync(m => m.OrderNumber, cancellationToken) 
                : 0;

            var movieStream = await Bus.RequestAsync(new GetMovieStreamQuery
            {
                MovieId = request.MovieId,
                TranslatorId = request.TranslatorId,
                Season = request.Season,
                Episode = request.Episode
            });

            if (movieStream == null || movieStream.Resolutions == null)
            {
                await Bus.InvokeDomainNotificationAsync("Cannot Fetch Movie Stream.");
                return Unit.Value;
            }

            var playlistMovie = await playlistMovieRepository
                .FirstOrDefaultAsync(pm => pm.RoomId == request.RoomId && pm.MovieId == request.MovieId,
                    cancellationToken);

            if (playlistMovie == null)
            {
                playlistMovie = new PlaylistMovie
                {
                    MovieId = request.MovieId,
                    RoomId = request.RoomId,
                    Name = movie.OriginalFullTitle,
                    OrderNumber = ++orderNumber,
                    VideoId = movie.VideoId,
                    VideoUrlId = movie.VideoUrlId
                };

                await playlistMovieRepository.AddAsync(playlistMovie, cancellationToken);
            }

            var playlistMovieVideo = new PlaylistMovieVideo
            {
                PlaylistMovieId = playlistMovie.Id,
                Season = request.Season,
                Episode = request.Episode,
                TranslatorExternalId = request.TranslatorId,
                TranslatorName = request.TranslatorName
            };
            await playlistMovieVideoRepository.AddAsync(playlistMovieVideo, cancellationToken);

            var isStreamFetched = false;

            foreach (var resolution in movieStream.Resolutions.Where(resolution => !string.IsNullOrEmpty(resolution.Link)))
            {
                isStreamFetched = true;
                
                var resolutionDefinition = resolution.ResolutionName switch
                {
                    "360p" => Shared.DTO.Enums.VideoQualityType.Low360,
                    "480p" => Shared.DTO.Enums.VideoQualityType.Medium480,
                    "720p" => Shared.DTO.Enums.VideoQualityType.High720,
                    "1080p" => Shared.DTO.Enums.VideoQualityType.Hd1080,
                    "1080p Ultra" => Shared.DTO.Enums.VideoQualityType.UltraHd1080,
                    _ => Shared.DTO.Enums.VideoQualityType.Low360
                };

                var playlistQuality = new PlaylistMovieVideoQuality()
                {
                    MovieUrl = resolution.Link,
                    PlaylistMovieId = playlistMovie.Id,
                    PlaylistMovieVideoId = playlistMovieVideo.Id,
                    QualityId = resolutionDefinition
                };
                await playlistMovieVideoQualityRepository.AddAsync(playlistQuality, cancellationToken);
            }

            if (!isStreamFetched)
            {
                await Bus.InvokeDomainNotificationAsync("Cannot Fetch Movie Stream.");
                return Unit.Value;
            }

            Commit();

            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteMovieFromPlaylistCommand request, CancellationToken cancellationToken)
        {
            if (IsUserAuthorized(user) == false)
            {
                await Bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            request
                .IsInvalidGuid(e => e.PlaylistMovieId, async () => await Bus.InvokeDomainNotificationAsync("RoomId is invalid."));

            var currentUser = await userRepository.GetAsync(user.Id.Value, cancellationToken);

            if (currentUser == null || currentUser.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("User is not authorized.");
                return Unit.Value;
            }

            if (!IsValidOperation())
            {
                return Unit.Value;
            }

            var playlistMovie = await playlistMovieRepository.GetAsync(request.PlaylistMovieId, cancellationToken);

            if (playlistMovie == null || playlistMovie.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("Playlist for Room doesn't exist.");
                return Unit.Value;
            }

            var roomUser = await roomUserRepository.FirstOrDefaultAsync(ru => ru.UserId == user.Id && ru.RoomId == playlistMovie.RoomId, cancellationToken);

            if (roomUser == null || roomUser.Id == Guid.Empty || !roomUser.IsOwner)
            {
                await Bus.InvokeDomainNotificationAsync("User doesn't have permission to use this room.");
                return Unit.Value;
            }
            
            var playlistMovieVideoQualities = await playlistMovieVideoQualityRepository
                .Query()
                .Where(q => q.PlaylistMovieId == playlistMovie.Id)
                .ToListAsync(cancellationToken);

            foreach (var playlistMovieVideoQuality in playlistMovieVideoQualities)
            {
                await playlistMovieVideoQualityRepository.DeleteAsync(playlistMovieVideoQuality.Id, cancellationToken);
            }

            Commit();

            var playlistMovieVideos = await playlistMovieVideoRepository
                .Query()
                .Where(q => q.PlaylistMovieId == playlistMovie.Id)
                .ToListAsync(cancellationToken);

            foreach (var playlistMovieVideo in playlistMovieVideos)
            {
                await playlistMovieVideoRepository.DeleteAsync(playlistMovieVideo.Id, cancellationToken);
            }

            Commit();

            await playlistMovieRepository.DeleteAsync(playlistMovie.Id, cancellationToken);

            Commit();

            return Unit.Value;
        }
    }
}
