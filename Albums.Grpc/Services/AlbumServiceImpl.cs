using AlbumsGrpc;
using Grpc.Core;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace AlbumsGrpc.Services
{
    // AlbumServiceImpl - our implementation of the service.
    // It inherits from AlbumService.AlbumServiceBase, a class that was
    // AUTO-GENERATED from album_service.proto when the project is built.
    public class AlbumServiceImpl : AlbumService.AlbumServiceBase
    {
        // In-memory storage (no database needed for this assignment).
        // ConcurrentDictionary is thread-safe, because many clients can call at the same time.
        // Key = album ID, Value = Album object
        private static readonly ConcurrentDictionary<string, Album> _albums = new();

        // Counter used to generate unique IDs ("1", "2", "3", ...)
        private static int _nextId = 0;

        // ---- Real-time updates (used by WatchAlbums) ----
        // Every connected client that is "watching" gets its own Subscriber.
        private class Subscriber
        {
            // A Channel is a thread-safe queue: AddAlbum/Update/Delete put events IN,
            // WatchAlbums takes them OUT and sends them to the client.
            public Channel<AlbumEvent> Events { get; } = Channel.CreateUnbounded<AlbumEvent>();

            // The client can change this filter at any time (empty = all genres)
            public volatile string GenreFilter = "";
        }

        private static readonly ConcurrentDictionary<Guid, Subscriber> _subscribers = new();

        // Static constructor: runs once and adds a few demo albums so the list is not empty
        static AlbumServiceImpl()
        {
            AddSeed("Abbey Road", "The Beatles", "Rock", 1969);
            AddSeed("Thriller", "Michael Jackson", "Pop", 1982);
            AddSeed("Kind of Blue", "Miles Davis", "Jazz", 1959);
        }

        private static void AddSeed(string title, string artist, string genre, int year)
        {
            var album = new Album
            {
                Id = Interlocked.Increment(ref _nextId).ToString(),
                Title = title,
                Artist = artist,
                Genre = genre,
                Year = year,
                Available = true
            };
            _albums[album.Id] = album;
        }

        // Sends an event ("Added", "Updated", "Deleted") to every watching client
        private static void Broadcast(string type, Album album)
        {
            var albumEvent = new AlbumEvent { Type = type, Album = album };

            foreach (var subscriber in _subscribers.Values)
            {
                string filter = subscriber.GenreFilter;

                // Skip this client if it asked only for another genre
                if (filter != "" && !string.Equals(filter, album.Genre, StringComparison.OrdinalIgnoreCase))
                    continue;

                subscriber.Events.Writer.TryWrite(albumEvent);
            }
        }

        // Shared validation used by Add and Update.
        // Throwing RpcException sends a proper gRPC error to the client.
        private static void Validate(string title, string artist, int year)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Title cannot be empty"));

            if (string.IsNullOrWhiteSpace(artist))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Artist cannot be empty"));

            if (year < 1900 || year > DateTime.Now.Year + 1)
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    $"Year must be between 1900 and {DateTime.Now.Year + 1}"));
        }

        // ------------------------------------------------------------
        // AddAlbum - creates a new album and adds it to the inventory
        // ------------------------------------------------------------
        public override Task<AddAlbumResponse> AddAlbum(AddAlbumRequest request, ServerCallContext context)
        {
            Validate(request.Title, request.Artist, request.Year);

            var album = new Album
            {
                Id = Interlocked.Increment(ref _nextId).ToString(),
                Title = request.Title.Trim(),
                Artist = request.Artist.Trim(),
                Genre = request.Genre.Trim(),
                Year = request.Year,
                Available = request.Available
            };

            _albums[album.Id] = album;

            Console.WriteLine($"[SERVER] Album added: ID={album.Id}, Title={album.Title}");
            Broadcast("Added", album);

            return Task.FromResult(new AddAlbumResponse { Album = album });
        }

        // ------------------------------------------------------------
        // GetAlbum - retrieves one album by its ID
        // ------------------------------------------------------------
        public override Task<GetAlbumResponse> GetAlbum(GetAlbumRequest request, ServerCallContext context)
        {
            if (!_albums.TryGetValue(request.Id, out var album))
                throw new RpcException(new Status(StatusCode.NotFound, $"Album with ID {request.Id} not found"));

            Console.WriteLine($"[SERVER] Album retrieved: ID={album.Id}, Title={album.Title}");

            return Task.FromResult(new GetAlbumResponse { Album = album });
        }

        // ------------------------------------------------------------
        // UpdateAlbum - changes the information of an existing album
        // ------------------------------------------------------------
        public override Task<UpdateAlbumResponse> UpdateAlbum(UpdateAlbumRequest request, ServerCallContext context)
        {
            if (!_albums.ContainsKey(request.Id))
                throw new RpcException(new Status(StatusCode.NotFound, $"Album with ID {request.Id} not found"));

            Validate(request.Title, request.Artist, request.Year);

            // Create the updated album (the ID stays the same)
            var updated = new Album
            {
                Id = request.Id,
                Title = request.Title.Trim(),
                Artist = request.Artist.Trim(),
                Genre = request.Genre.Trim(),
                Year = request.Year,
                Available = request.Available
            };

            _albums[request.Id] = updated;

            Console.WriteLine($"[SERVER] Album updated: ID={updated.Id}, Title={updated.Title}");
            Broadcast("Updated", updated);

            return Task.FromResult(new UpdateAlbumResponse { Album = updated });
        }

        // ------------------------------------------------------------
        // DeleteAlbum - removes an album from the inventory
        // ------------------------------------------------------------
        public override Task<DeleteAlbumResponse> DeleteAlbum(DeleteAlbumRequest request, ServerCallContext context)
        {
            if (!_albums.TryRemove(request.Id, out var removed))
                throw new RpcException(new Status(StatusCode.NotFound, $"Album with ID {request.Id} not found"));

            Console.WriteLine($"[SERVER] Album deleted: ID={removed.Id}, Title={removed.Title}");
            Broadcast("Deleted", removed);

            return Task.FromResult(new DeleteAlbumResponse
            {
                Success = true,
                Message = $"Album '{removed.Title}' successfully deleted"
            });
        }

        // ------------------------------------------------------------
        // ListAlbums - SERVER streaming: sends the albums one by one
        // ------------------------------------------------------------
        public override async Task ListAlbums(ListAlbumsRequest request, IServerStreamWriter<Album> responseStream, ServerCallContext context)
        {
            Console.WriteLine($"[SERVER] Streaming all albums. Total count: {_albums.Count}");

            // Sort by numeric ID so the list always has the same order
            foreach (var album in _albums.Values.OrderBy(a => int.Parse(a.Id)))
            {
                // Stop if the client cancelled the request
                if (context.CancellationToken.IsCancellationRequested)
                    break;

                await responseStream.WriteAsync(album);
            }
        }

        // ------------------------------------------------------------
        // WatchAlbums - BIDIRECTIONAL streaming (real-time updates)
        //   client -> server : WatchRequest (genre filter, can be sent many times)
        //   server -> client : AlbumEvent   (pushed whenever something changes)
        // Both streams are open at the same time until the client disconnects.
        // ------------------------------------------------------------
        public override async Task WatchAlbums(
            IAsyncStreamReader<WatchRequest> requestStream,
            IServerStreamWriter<AlbumEvent> responseStream,
            ServerCallContext context)
        {
            var id = Guid.NewGuid();
            var subscriber = new Subscriber();
            _subscribers[id] = subscriber;
            Console.WriteLine($"[SERVER] A client started watching. Watchers: {_subscribers.Count}");

            // Part 1: read the client's messages in the background (filter changes)
            var readTask = Task.Run(async () =>
            {
                try
                {
                    await foreach (var message in requestStream.ReadAllAsync(context.CancellationToken))
                        subscriber.GenreFilter = message.GenreFilter?.Trim() ?? "";
                }
                catch (OperationCanceledException) { /* client disconnected - fine */ }
                catch (IOException) { /* connection dropped - fine */ }
            });

            // Part 2: send events to the client as soon as they appear in the queue
            try
            {
                await foreach (var albumEvent in subscriber.Events.Reader.ReadAllAsync(context.CancellationToken))
                    await responseStream.WriteAsync(albumEvent);
            }
            catch (OperationCanceledException) { /* client disconnected - fine */ }
            finally
            {
                // Always clean up, so we never send events to a client that is gone
                _subscribers.TryRemove(id, out _);
                Console.WriteLine($"[SERVER] A client stopped watching. Watchers: {_subscribers.Count}");
            }

            await readTask;
        }
    }
}
