using System;
using Kozachok.Shared.Abstractions.Commands;

namespace Kozachok.Domain.Commands.Playlist
{
    public class AddMovieToPlaylistCommand : Command
    {
        public Guid RoomId { get; set; }
        public Guid MovieId { get; set; }
        public string TranslatorId { get; set; }
        public int Season { get; set; }
        public int Episode { get; set; }
    }
}
