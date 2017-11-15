using System;

namespace Client
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Gameworld.startServer(6666);

            using (var game = new Gameworld())
                game.Run();
        }
    }
#endif
}
