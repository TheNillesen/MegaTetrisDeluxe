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
            Gameworld.startServer(6666, 50, 50, 1);

            using (var game = Gameworld.Instance)
                game.Run();
        }
    }
#endif
}
