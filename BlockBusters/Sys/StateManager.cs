/***** State Manager *****/

namespace BlockBusters.Sys {

    #region Enumerators

    /// <summary>
    /// Enumerator that tracks the current state of the game.
    /// </summary>
    public enum GameState {
        Splash,
        Main_Menu,
        Game_Running,
        Game_Paused,
        Credits,
        Exit
    }

    #endregion

    #region Objects

    /// <summary>
    /// Class that manages states of the game.
    /// </summary>
    public static class StateManager {

        #region Declarations

        public static GameState gameState;

        #endregion
    }

    #endregion
}
