namespace AlbumsUI
{
    internal static class Program
    {
        // Entry point of the stand-alone Albums app.
        // Later, the VibeHive main menu will simply call:  new AlbumsForm().Show();
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new AlbumsForm());
        }
    }
}
