/***** GLOBAL DEFINITIONS *****/

#region Prerequisites

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace BlockBusters.Main {

    #region Objects

    public class Textures {

        #region Declarations

        public static Texture2D tex_SplashBg,
                                tex_HexSelector,
                                tex_MainMenuTitle,
                                tex_MainMenuBg,
                                tex_MainMenuSpinner,
                                tex_BlueExlosion,
                                tex_InGameBg,
                                tex_InfoContainer,
                                tex_qaContainer,
                                tex_DBG_GPIC1,
                                tex_DBG_GPIC2,
                                tex_Dummy;

        #endregion
    }

    public class Sounds {

        #region Declarations

        public static SoundEffect sfx_Splash,
                                  sfx_MenuSweep,
                                  sfx_MenuTrack;

        #endregion
    }

    public class Fonts {

        #region Declarations

        public static SpriteFont font_BoardLetters,
                                 font_MainMenu;

        #endregion
    }

    #endregion
}