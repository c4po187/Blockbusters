#region Prerequisites

using System;
using BlockBusters.Main;

#endregion

namespace BlockBusters.Sys {

#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program {
        
        #region Entry Point

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }

        #endregion
    }
#endif
}
