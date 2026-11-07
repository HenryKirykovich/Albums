# Module 4 - Album Inventory Microservice (gRPC + WinForms)

## Projects
- `Albums.Grpc` - gRPC server, http://localhost:7300 (HTTP/2), in-memory storage, 3 demo albums
- `Albums.UI`   - WinForms client (`AlbumsForm`), uses the SAME `.proto` file (linked from Albums.Grpc/Protos)

## How to run (Visual Studio)
1. Right-click the solution -> Properties -> Common Properties -> Configure Startup Projects -> **Multiple startup projects**
   -> set both `Albums.Grpc` and `Albums.UI` to **Start** (Albums.Grpc first in the list).
2. Press F5. A console window (server) and the Albums window open.
   The client retries the connection, so the start order is not critical.

## gRPC methods (album_service.proto)
| Method | Type | What it does |
|---|---|---|
| AddAlbum | unary | adds an album (Title + Artist required, Year 1900..next year) |
| GetAlbum | unary | gets one album by ID (NotFound error if missing) |
| UpdateAlbum | unary | changes an album |
| DeleteAlbum | unary | deletes an album |
| ListAlbums | server streaming | streams all albums one by one |
| WatchAlbums | bidirectional streaming | server pushes Added/Updated/Deleted events in real time; client can send a genre filter |

Album fields: Id, Title, Artist, Genre, Year, Available.

## Real-time demo
Run two copies of `Albums.UI` (start the second from the `bin` folder). Add an album in one window -
it appears in the other one within a moment ("Live updates: ON" in the status bar).

## Joining the main VibeHive menu later
Keep `Albums.UI` as it is and call `new AlbumsForm().Show();` from the menu button
(add a project reference to Albums.UI). Port 7300 is the agreed port for this module.
