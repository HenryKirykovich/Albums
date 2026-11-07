using AlbumsGrpc;

namespace AlbumsUI
{
    // One row of the table on the screen.
    // We copy the gRPC "Album" message into this simple class, because
    // DataGridView shows the public properties of a class as columns.
    public class AlbumRow
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Artist { get; set; } = "";
        public string Genre { get; set; } = "";
        public int Year { get; set; }
        public bool Available { get; set; }

        public static AlbumRow From(Album album)
        {
            return new AlbumRow
            {
                Id = album.Id,
                Title = album.Title,
                Artist = album.Artist,
                Genre = album.Genre,
                Year = album.Year,
                Available = album.Available
            };
        }
    }
}
