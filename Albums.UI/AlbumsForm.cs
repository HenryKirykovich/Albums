using AlbumsGrpc;
using Grpc.Core;
using Grpc.Net.Client;

namespace AlbumsUI
{
    // AlbumsForm - the Windows Forms screen for the Album Inventory module.
    // It talks to the Albums.Grpc server (Add / Update / Delete / List + live updates).
    //
    // This is the "code" half of the form. The "design" half (where the buttons, text boxes
    // and the table are placed) is in AlbumsForm.Designer.cs -
    // double-click AlbumsForm.cs in Solution Explorer to edit the screen visually.
    public partial class AlbumsForm : Form
    {
        // Address of the gRPC server (must match appsettings.json in Albums.Grpc)
        private const string ServerAddress = "http://localhost:7300";

        // ---- gRPC objects ----
        private GrpcChannel? _channel;                       // the connection (created once, reused)
        private AlbumService.AlbumServiceClient? _client;    // generated from the .proto file

        // Used to stop the background "live updates" loop when the form closes
        private readonly CancellationTokenSource _cts = new();

        // Makes sure only ONE refresh runs at a time
        private readonly SemaphoreSlim _refreshLock = new(1, 1);

        // ID of the album the user clicked in the table (null = nothing selected)
        private string? _selectedId;

        // True while we are re-filling the table (so we ignore selection events)
        private bool _binding;

        public AlbumsForm()
        {
            InitializeComponent();   // creates all the controls (see AlbumsForm.Designer.cs)

            // Values that depend on today's date can't be set in the designer, so we set them here
            nudYear.Maximum = DateTime.Now.Year + 1;   // same rule as the server
            nudYear.Value = DateTime.Now.Year;
        }

        // =====================================================================
        //  Form events (the designer connects these to the form)
        // =====================================================================

        // Start-up: connect and start the live updates
        private async void AlbumsForm_Load(object? sender, EventArgs e)
        {
            // Creating the channel does not connect yet - the first call does.
            _channel = GrpcChannel.ForAddress(ServerAddress);
            _client = new AlbumService.AlbumServiceClient(_channel);

            await RefreshAlbumsAsync();   // first load of the table
            await WatchLoopAsync();       // then listen for live updates (runs until the form closes)
        }

        private void AlbumsForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            _cts.Cancel();   // stops the live-updates loop
        }

        private void AlbumsForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
            _channel?.Dispose();
        }

        // Button clicks
        private async void btnAdd_Click(object? sender, EventArgs e) => await AddAlbumAsync();
        private async void btnUpdate_Click(object? sender, EventArgs e) => await UpdateAlbumAsync();
        private async void btnDelete_Click(object? sender, EventArgs e) => await DeleteAlbumAsync();
        private async void btnRefresh_Click(object? sender, EventArgs e) => await RefreshAlbumsAsync();
        private void btnClear_Click(object? sender, EventArgs e) => ClearFields();

        // When the user clicks a row, copy its values into the input fields
        private void dgvAlbums_SelectionChanged(object? sender, EventArgs e)
        {
            if (_binding || dgvAlbums.SelectedRows.Count == 0) return;

            if (dgvAlbums.SelectedRows[0].DataBoundItem is AlbumRow row)
            {
                _selectedId = row.Id;
                txtTitle.Text = row.Title;
                txtArtist.Text = row.Artist;
                txtGenre.Text = row.Genre;
                nudYear.Value = Math.Clamp(row.Year, (int)nudYear.Minimum, (int)nudYear.Maximum);
                chkAvailable.Checked = row.Available;
            }
        }

        // =====================================================================
        //  Live updates (bidirectional streaming)
        // =====================================================================
        private async Task WatchLoopAsync()
        {
            // Loop forever: if the server is down or restarts, wait 2 seconds and reconnect.
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    using var call = _client!.WatchAlbums(cancellationToken: _cts.Token);

                    // client -> server: our first message (empty filter = "send me everything")
                    await call.RequestStream.WriteAsync(new WatchRequest { GenreFilter = "" });

                    SetLive(true);
                    await RefreshAlbumsAsync();   // catch up on anything we missed while disconnected

                    // server -> client: wait for events pushed by the server
                    await foreach (var albumEvent in call.ResponseStream.ReadAllAsync(_cts.Token))
                    {
                        SetStatus($"Live update: '{albumEvent.Album.Title}' was {albumEvent.Type.ToLower()}");
                        await RefreshAlbumsAsync();
                    }
                }
                catch (OperationCanceledException)
                {
                    break;   // the form is closing
                }
                catch (RpcException)
                {
                    // Server not reachable (or the stream was cancelled) - try again below
                }

                if (_cts.IsCancellationRequested) break;

                SetLive(false);

                try { await Task.Delay(2000, _cts.Token); }
                catch (OperationCanceledException) { break; }
            }
        }

        // =====================================================================
        //  Loading the table (server streaming)
        // =====================================================================
        private async Task RefreshAlbumsAsync()
        {
            if (_client == null) return;

            await _refreshLock.WaitAsync();
            try
            {
                var rows = new List<AlbumRow>();

                // ListAlbums is a SERVER STREAM: the albums arrive one by one
                using var call = _client.ListAlbums(new ListAlbumsRequest(), cancellationToken: _cts.Token);
                await foreach (var album in call.ResponseStream.ReadAllAsync(_cts.Token))
                    rows.Add(AlbumRow.From(album));

                ShowRows(rows);
                SetStatus($"Connected - {rows.Count} album(s) in the inventory");
            }
            catch (OperationCanceledException) { /* form closing */ }
            catch (RpcException ex)
            {
                if (!_cts.IsCancellationRequested)
                    SetStatus(DescribeError(ex));
            }
            finally
            {
                _refreshLock.Release();
            }
        }

        // Puts the albums into the table and keeps the user's selection
        private void ShowRows(List<AlbumRow> rows)
        {
            if (IsDisposed) return;

            _binding = true;   // ignore selection events while we rebuild the table
            try
            {
                dgvAlbums.DataSource = rows;
                dgvAlbums.ClearSelection();

                // Re-select the album the user had selected (if it still exists)
                bool stillThere = false;
                foreach (DataGridViewRow row in dgvAlbums.Rows)
                {
                    if (row.DataBoundItem is AlbumRow albumRow && albumRow.Id == _selectedId)
                    {
                        row.Selected = true;
                        stillThere = true;
                        break;
                    }
                }

                if (!stillThere) _selectedId = null;
            }
            finally
            {
                _binding = false;
            }
        }

        // =====================================================================
        //  Add / Update / Delete
        // =====================================================================
        private async Task AddAlbumAsync()
        {
            if (!InputIsValid()) return;

            try
            {
                var response = await _client!.AddAlbumAsync(new AddAlbumRequest
                {
                    Title = txtTitle.Text.Trim(),
                    Artist = txtArtist.Text.Trim(),
                    Genre = txtGenre.Text.Trim(),
                    Year = (int)nudYear.Value,
                    Available = chkAvailable.Checked
                });

                SetStatus($"Album added with ID {response.Album.Id}");
                ClearFields();
                await RefreshAlbumsAsync();
            }
            catch (RpcException ex)
            {
                ShowError(ex);
            }
        }

        private async Task UpdateAlbumAsync()
        {
            if (_selectedId == null)
            {
                MessageBox.Show("Please click an album in the table first.", "Update",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!InputIsValid()) return;

            try
            {
                var response = await _client!.UpdateAlbumAsync(new UpdateAlbumRequest
                {
                    Id = _selectedId,
                    Title = txtTitle.Text.Trim(),
                    Artist = txtArtist.Text.Trim(),
                    Genre = txtGenre.Text.Trim(),
                    Year = (int)nudYear.Value,
                    Available = chkAvailable.Checked
                });

                SetStatus($"Album {response.Album.Id} updated");
                await RefreshAlbumsAsync();
            }
            catch (RpcException ex)
            {
                ShowError(ex);
            }
        }

        private async Task DeleteAlbumAsync()
        {
            if (_selectedId == null)
            {
                MessageBox.Show("Please click an album in the table first.", "Delete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var answer = MessageBox.Show($"Delete the album with ID {_selectedId}?", "Confirm delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer != DialogResult.Yes) return;

            try
            {
                var response = await _client!.DeleteAlbumAsync(new DeleteAlbumRequest { Id = _selectedId });

                SetStatus(response.Message);
                _selectedId = null;
                ClearFields();
                await RefreshAlbumsAsync();
            }
            catch (RpcException ex)
            {
                ShowError(ex);
            }
        }

        // =====================================================================
        //  Small helpers
        // =====================================================================

        // Simple input validation: Title and Artist must be filled in
        private bool InputIsValid()
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtArtist.Text))
            {
                MessageBox.Show("Title and Artist are required.", "Missing data",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void ClearFields()
        {
            txtTitle.Clear();
            txtArtist.Clear();
            txtGenre.Clear();
            nudYear.Value = Math.Min(DateTime.Now.Year, nudYear.Maximum);
            chkAvailable.Checked = true;
            _selectedId = null;
            dgvAlbums.ClearSelection();
        }

        private void SetStatus(string text)
        {
            if (!IsDisposed) lblStatus.Text = text;
        }

        private void SetLive(bool on)
        {
            if (!IsDisposed) lblLive.Text = on ? "Live updates: ON" : "Live updates: OFF";
        }

        // Turns a gRPC error into a message a normal person can read
        private static string DescribeError(RpcException ex)
        {
            if (ex.StatusCode == StatusCode.Unavailable)
                return $"Cannot reach the server at {ServerAddress}. Is Albums.Grpc running?";

            return $"{ex.StatusCode}: {ex.Status.Detail}";
        }

        private void ShowError(RpcException ex)
        {
            MessageBox.Show(DescribeError(ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
