#region Prerequisites

using System;
using System.Collections.Generic;
using BlockBusters.Graphics;
using BlockBusters.Main;
using BlockBusters.Sys;
using EUMD_CS.Graphics.GeometryPrimitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace BlockBusters.UI {

    /*
     * - Main background
     * - Play Game (selective) -> Sub menu
     * - Options (selective) -> Sub menu
     */

    #region Enumerators

    /// <summary>
    /// Enumerator that tracks the current styate of the Main Menu.
    /// </summary>
    public enum MenuState { 
        Opt_transition,
        Active,
        Inactive
    }

    #endregion

    #region Objects

    /// <summary>
    /// Class that defines the Game's Main Menu.
    /// </summary>
    public class MainMenu : Menu {

        #region Constructors

        /// <summary>
        /// Constructs an instance of MainMenu with an 
        /// animated background if desired.
        /// </summary>
        /// <param name="gfxDevice">
        /// Parameter represents the current Graphics Device (GPU).
        /// </param>
        /// <param name="background">
        /// A texture that displays an image for the menu background.
        /// </param>
        public MainMenu(GraphicsDevice gfxDevice, Texture2D background) {
            m_gfxDevice = gfxDevice;
            m_background = background;

            // TODO: Set this properly when we ready
            m_selector = new Selector {
                Bounds = new Oblong(Point.Zero, 1, 1, Color.Crimson, m_gfxDevice),
                Visible = false
            };

            // Init lists
            m_animations = new List<Animated>();
            m_selectives = new List<Selective>();
            m_children = new List<Menu>();

            // Init Spinner
            m_spinner = new Animated(Textures.tex_MainMenuSpinner, 8, 0, new Vector2(100f, 300f));
            m_spinner.TotalRows = 2;
            m_spinner.Visibility = false;

            // Set Members
            this.Active = m_bIsSweeping = m_bPlayTrack = true;
            m_trackInstance = Sounds.sfx_MenuTrack.CreateInstance();
            m_trackInstance.IsLooped = true;
            m_trackInstance.Volume = 0.5f;
            m_menuState = MenuState.Opt_transition;
            m_transIndex = 0;
            m_timer = 0.0;
        }

        #endregion

        #region Declarations

        private Texture2D m_background;
        private GraphicsDevice m_gfxDevice;
        private SoundEffectInstance m_trackInstance;
        private Animated m_spinner;
        private MenuState m_menuState;
        private int m_transIndex;
        private bool m_bIsSweeping, m_bPlayTrack;
        private double m_timer;

        #endregion

        #region Properties
        #endregion

        #region Functions

        /// <summary>
        /// Chaining function.
        /// Initializes all the selectives within this menu.
        /// </summary>
        /// <param name="selectedColour">
        /// Represents the colour to alternate to, once a selective is selected (nullable).
        /// </param>
        /// <param name="selectives">
        /// Represents an indefinite number of Selectives, that may be included.
        /// </param>
        /// <returns>
        /// Returns the current instance of MainMenu.
        /// </returns>
        public MainMenu initSelectives(Color? selectedColour, params Selective[] selectives) {
            if (selectedColour.HasValue)
                m_hoverColour = selectedColour.Value;

            foreach (Selective selective in selectives) {
                m_selectives.Add(selective);
                if (selective.Child != null)
                    assignChildren(selective.Child);
            }

            return this;
        }

        /// <summary>
        /// Chaining Function.
        /// Initializes all the Animations within this menu.
        /// </summary>
        /// <param name="animations">
        /// Represents an indefinite number of Animations, that may be included.
        /// </param>
        /// <returns>
        /// Returns the current instance of MainMenu.
        /// </returns>
        public MainMenu initAnimations(params Animated[] animations) {
            foreach (Animated animation in animations) {
                animation.Visibility = true;
                m_animations.Add(animation);
            }

            return this;
        }

        /// <summary>
        /// Function that simply adds an instance of menu to the list of children.
        /// </summary>
        /// <param name="child">
        /// Parameter represents a child (sub-menu) of this menu.
        /// </param>
        private void assignChildren(Menu child) {
            m_children.Add(child);
        }

        /// <summary>
        /// Updates the logic invlolved with the Main Menu. (Overridden).
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of timing values.
        /// </param>
        /// <param name="inputManager">
        /// Provides the user access to input devices, capturing their events.
        /// </param>
        public override void update(GameTime gameTime, InputManager inputManager) {
            // Delay for Menu Track
            double delay = .425;

            // Set Title FPS
            m_animations[0].FramesPerSecond = 8.0;

            switch (m_menuState) { 
                case MenuState.Opt_transition: 
                    if (m_selectives[m_transIndex].Container.X < 320.0f) {
                        Rectangle current = m_selectives[m_transIndex].Container;
                        current.X += (int)gameTime.ElapsedGameTime.TotalMilliseconds * 2;
                        m_selectives[m_transIndex].Container = current;
                    }
                    else if (m_transIndex < 2) {
                        ++m_transIndex;
                        m_bIsSweeping = true;
                    }
                    else
                        m_menuState = MenuState.Active;

                    if (m_bIsSweeping) {
                        Sounds.sfx_MenuSweep.Play(0.3f, 0.7f, 0f);
                        m_bIsSweeping = false;
                    }
                    break;
                case MenuState.Active:
                    m_timer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (m_bPlayTrack && (m_timer > delay)) {
                        m_trackInstance.Play();
                        m_bPlayTrack = false;
                    }

                    if (m_selectives[0].Selected) {
                        m_trackInstance.Stop();
                        m_trackInstance.Dispose();
                        StateManager.gameState = GameState.Game_Running;
                    }
                    if (m_selectives[2].Selected)
                        StateManager.gameState = GameState.Exit;

                    foreach (Selective s in m_selectives) {
                        if (!s.Hover)
                            m_spinner.Visibility = false;
                        else {
                            m_spinner.Position = new Vector2(
                                (float)(s.Container.X - 50), (float)(s.Container.Y + 20));
                            m_spinner.Visibility = true;
                            break;
                        }
                    }

                    // Update Spinner
                    m_spinner.updateAnimation(gameTime);
                    break;
                case MenuState.Inactive:
                    break;
            }

            // Update base class
            base.update(gameTime, inputManager);
        }

        public override void draw(SpriteBatch spriteBatch) {
            // Draw Background
            spriteBatch.Draw(
                m_background, new Rectangle(0, 0, m_background.Width, m_background.Height), Color.White);

            // Draw base elements
            base.draw(spriteBatch);

            // Draw small texture to cap title overflow
            Textures.tex_Dummy.SetData(new Color[] { Color.Black });
            spriteBatch.Draw(Textures.tex_Dummy,
                new Rectangle(m_animations[0].DestinationRectangle.Right - 5,
                    m_animations[0].DestinationRectangle.Top, 10, m_animations[0].SpriteSheet.Height),
                    Color.White);

            // Draw Spinner
            if (m_spinner.Visibility)
                m_spinner.draw(spriteBatch);
        }

        #endregion
    }

    #endregion
}
