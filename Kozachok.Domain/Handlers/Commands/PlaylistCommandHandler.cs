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
using System.Linq;
using Kozachok.Domain.Queries.MovieCatalog;
using Kozachok.Shared;
using Microsoft.EntityFrameworkCore;

namespace Kozachok.Domain.Handlers.Commands
{
    public class PlaylistCommandHandler :
        CommandHandler,
        IRequestHandler<AddMovieToPlaylistCommand>,
        IRequestHandler<DeleteMovieFromPlaylistCommand>
    {
        private readonly IPlaylistMovieRepository playlistMovieRepository;
        private readonly IPlaylistMovieVideoRepository playlistMovieVideoRepository;
        private readonly IPlaylistMovieVideoQualityRepository playlistMovieVideoQualityRepository;
        private readonly IMovieRepository movieRepository;
        private readonly IRoomRepository roomRepository;
        private readonly IUser user;

        public PlaylistCommandHandler(
            IUnitOfWork uow,
            IMediatorHandler bus,
            INotificationHandler<DomainNotification> notifications,
            IPlaylistMovieRepository playlistMovieRepository,
            IPlaylistMovieVideoRepository playlistMovieVideoRepository,
            IPlaylistMovieVideoQualityRepository playlistMovieVideoQualityRepository,
            IMovieRepository movieRepository,
            IRoomRepository roomRepository,
            IUser user)
        : base(
                uow,
                bus,
                notifications
        )
        {
            this.playlistMovieRepository = playlistMovieRepository;
            this.playlistMovieVideoRepository = playlistMovieVideoRepository;
            this.playlistMovieVideoQualityRepository = playlistMovieVideoQualityRepository;
            this.movieRepository = movieRepository;
            this.roomRepository = roomRepository;
            this.user = user;
        }

        public async Task<Unit> Handle(AddMovieToPlaylistCommand request, CancellationToken cancellationToken)
        {
            var isUserHasPermissionToAddMovie = await roomRepository
                .AnyAsync(r => r.Id == request.RoomId && r.OwnerUserId == user.Id, cancellationToken);

            if (!isUserHasPermissionToAddMovie)
            {
                await Bus.InvokeDomainNotificationAsync("User doesn't have permission to add movie to this room.");
                return Unit.Value;
            }

            var playlistMoviesCount = await playlistMovieRepository
                .Query(pm => pm.RoomId == request.RoomId)
                .CountAsync(cancellationToken);

            if (playlistMoviesCount >= GlobalConstants.PlaylistRoomMaxSize)
            {
                await Bus.InvokeDomainNotificationAsync("You have exceeded the limit for adding movies to the page, please delete the existing titles in your playlist to continue.");
                return Unit.Value;
            }

            var movie = await movieRepository.GetAsync(request.MovieId, cancellationToken);

            if (movie == null || movie.Id == Guid.Empty)
            {
                await Bus.InvokeDomainNotificationAsync("Movie doesn't exist.");
                return Unit.Value;
            }

            var orderNumber = await playlistMovieRepository
                    .Query()
                    .Where(m => m.RoomId == request.RoomId)
                    .MaxAsync(m => m.OrderNumber, 
                        cancellationToken);

            var movieStream = await Bus.RequestAsync(new GetMovieStreamQuery
            {
                MovieId = request.MovieId,
                TranslatorId = request.TranslatorId,
                Season = request.Season,
                Episode = request.Episode
            });

            if (movieStream?.Resolutions == null)
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

            var playlistMovie = await playlistMovieRepository
                .Query(q => q.Id == request.PlaylistMovieId)
                .Include(q => q.Room)
                .Include(q => q.PlaylistMovieVideos)
                    .ThenInclude(q => q.PlaylistMovieVideoQualities)
                .SingleOrDefaultAsync(cancellationToken);

            var isUserHasPermissionToRemoveMovie = playlistMovie != null && playlistMovie.Room.OwnerUserId == user.Id;

            if (!isUserHasPermissionToRemoveMovie)
            {
                await Bus.InvokeDomainNotificationAsync("User doesn't have permission to use this room.");
                return Unit.Value;
            }

            playlistMovieVideoQualityRepository.DeleteRange(playlistMovie.PlaylistMovieVideos
                    .SelectMany(q => q.PlaylistMovieVideoQualities));

            playlistMovieVideoRepository.DeleteRange(playlistMovie.PlaylistMovieVideos);

            playlistMovieRepository.Delete(playlistMovie);

            Commit();

            return Unit.Value;
        }
    }
}
