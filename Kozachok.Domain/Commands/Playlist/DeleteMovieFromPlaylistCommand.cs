using Kozachok.Shared.Abstractions.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kozachok.Domain.Commands.Playlist
{
    public class DeleteMovieFromPlaylistCommand : Command
    {
        public Guid PlaylistMovieId { get; set; }
    }
}
