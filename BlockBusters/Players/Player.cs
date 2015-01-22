#region Prerequisites

using System;
using BlockBusters.Main;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

// TODO: Need to implement Text Input enabling players to set their own names.

namespace BlockBusters.Players {

    #region Enumerators

    /// <summary>
    /// Enumerator that tracks when its the current players turn.
    /// </summary>
    public enum TurnSpecifier { 
        Waiting,
        Engaging
    }

    /// <summary>
    /// Enumerator that tracks the type of win of the player.
    /// </summary>
    public enum WinType { 
        Single,
        Round
    }

    #endregion

    #region Objects

    /// <summary>
    /// Base Player class, that represents a human or CPU player.
    /// </summary>
    public abstract class Player {

        #region Declarations

        protected string m_name;
        protected Texture2D m_gamerPic;
        protected Vector2 m_position;
        protected TurnSpecifier m_turnSpec;
        protected WinType m_winType;
        protected bool m_bIsCorrect;

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the Turn Specifier.
        /// </summary>
        public TurnSpecifier TurnSpec {
            get { return m_turnSpec; }
            set { m_turnSpec = value; }
        }

        /// <summary>
        /// Gets and sets the Win Type.
        /// </summary>
        public WinType PWinType {
            get { return m_winType; }
            set { m_winType = value; }
        }

        /// <summary>
        /// Gets the name of the player.
        /// </summary>
        public string Name {
            get { return m_name; }
        }

        /// <summary>
        /// Gets and sets the position of the player's
        /// name and gamer picture.
        /// </summary>
        public Vector2 Position {
            get { return m_position; }
            set { m_position = value; }
        }

        /// <summary>
        /// Gets and sets the correct boolean.
        /// </summary>
        public bool Correct {
            get { return m_bIsCorrect; }
            set { m_bIsCorrect = value; }
        }

        /// <summary>
        /// Gets and sets the player's score.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Gets and sets the player's consecutive correct answers.
        /// </summary>
        public int Consecutive { get; set; }

        /// <summary>
        /// Gets and sets a boolean which indicates whether the players info
        /// should be presented in reverse order.
        /// </summary>
        public bool ReverseLayout { get; set; }

        #endregion

        #region Functions

        /// <summary>
        /// Updates the logic of the Player.
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of timing values.
        /// </param>
        public virtual void update(GameTime gameTime) {
            if (m_bIsCorrect && m_turnSpec.HasFlag(TurnSpecifier.Engaging)) {
                switch (m_winType) {
                    case WinType.Single:
                        Score += 50;
                        ++Consecutive;
                        break;
                    case WinType.Round:
                        Score += 500;
                        break;
                }
            } else 
                Consecutive = 0;

            m_bIsCorrect = false;
            m_turnSpec = TurnSpecifier.Waiting;
        }

        /// <summary>
        /// Renders the Players assets to the screen.
        /// </summary>
        /// <param name="spriteBatch">
        /// Parameter represents the spritebatch, used
        /// for drawing textures and alike.
        /// </param>
        public virtual void draw(SpriteBatch spriteBatch) {
            // TODO : Set condition here and new DrawString for reverse layout
            if (ReverseLayout) {
                spriteBatch.Draw(m_gamerPic, new Rectangle(
                (int)(m_position.X - m_gamerPic.Width), (int)m_position.Y, m_gamerPic.Width, m_gamerPic.Height), 
                Color.White);
                spriteBatch.DrawString(Fonts.font_MainMenu, m_name, new Vector2(
                    (float)(m_position.X - m_gamerPic.Width - 20 - (2f * Fonts.font_MainMenu.MeasureString(m_name).X)), 
                    (m_position.Y - 10f)), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            } else {
                spriteBatch.Draw(m_gamerPic, new Rectangle(
                (int)m_position.X, (int)m_position.Y, m_gamerPic.Width, m_gamerPic.Height), Color.White);
                spriteBatch.DrawString(Fonts.font_MainMenu, m_name, new Vector2(
                    (float)(m_position.X + m_gamerPic.Width + 20), (m_position.Y - 10f)), Color.White, 0f,
                    Vector2.Zero, 2f, SpriteEffects.None, 0f);
            }
        }

        #endregion
    }

    #endregion
}
