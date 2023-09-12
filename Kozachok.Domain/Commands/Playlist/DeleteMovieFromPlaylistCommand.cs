using Kozachok.Shared.Abstractions.Commands;
using System;

namespace Kozachok.Domain.Commands.Playlist
{
    public class DeleteMovieFromPlaylistCommand : Command
    {
        public Guid PlaylistMovieId { get; set; }
    }
}
