using System;

namespace GameBoyMono
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
        static void Main(string[] parameter)
        {
            Game1.parameter = parameter;

            using (var game = new Game1())
                game.Run();
        }
    }
#endif
}
