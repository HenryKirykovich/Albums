using AlbumsGrpc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add gRPC services to the DI container
builder.Services.AddGrpc();

var app = builder.Build();

// Map our AlbumService so it can receive incoming gRPC calls
app.MapGrpcService<AlbumServiceImpl>();

// Default route - a browser can't talk gRPC, so show a friendly message instead
app.MapGet("/", () => "gRPC Album Inventory server is running. Use a gRPC client to connect.");

// The port (7300) is set in appsettings.json -> Kestrel -> Endpoints
Console.WriteLine("Album Inventory gRPC server starting on http://localhost:7300 ...");

app.Run();
