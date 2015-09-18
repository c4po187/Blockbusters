#region Prerequisites

using System;
using System.Web.UI;
using BlockBusters.Data;
using BlockBusters.Graphics;
using BlockBusters.Players;
using BlockBusters.Sys;
using BlockBusters.UI;
using EUMD_CS.Graphics;
using EUMD_CS.Graphics.GeometryPrimitives;
using EUMD_CS.Graphics.Imaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;


#endregion

// TODO: Mouse Trailing Particles

namespace BlockBusters.Main {

    #region Objects

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BlockBusters_Game : Game {

        #region Declarations

        GraphicsDeviceManager g_graphics;
        SpriteBatch g_spriteBatch;
        InputManager g_inputManager;
        MainMenu g_mainMenu;    
        Oblong g_oblong;
        Board g_board;
        HexSelector g_hexSelector;
        Fader g_fader;
        public static QA_Compiler gs_QAComp;
        Vector2 g_baseScreenSize;
        Player[] g_players;
        bool g_bIsIntroStarted;

        #endregion

        #region Constructors

        public BlockBusters_Game()
            : base() {
            g_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            StateManager.gameState = GameState.Splash;
            g_bIsIntroStarted = false;
            #if WINDOWS || LINUX
                IsMouseVisible = true;
            #endif
        }

        #endregion

        #region Functions

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here
            g_inputManager = new InputManager();

            // Set the screen resolution as default to WXGA_H 720p
            Resolution startUpResolution = CommonResolutions.WXGA_H;
            g_graphics.PreferredBackBufferWidth = startUpResolution.width;
            g_graphics.PreferredBackBufferHeight = startUpResolution.height;
            g_graphics.ApplyChanges();

            // Graphics are uniform following resolution (720p) --> scaling factor = 1
            g_baseScreenSize = startUpResolution;

            // Compile questions from XML
            gs_QAComp = new QA_Compiler().
                setReader(@"Content\XML\QA\blockbusters_geography.xml").readDataToList().
                setReader(@"Content\XML\QA\blockbusters_general-knowledge.xml").readDataToList().
                setReader(@"Content\XML\QA\blockbusters_sports.xml").readDataToList();

            // Init a tile for our board
            Tile mainTile = new Tile {
                spriteSheet = Content.Load<Texture2D>(@"Textures\bb_hex_alt.png"),
                width = 100,
                height = 82,
                stepX = 160,
                stepY = 43,
                evenRow_offsetX = 80
            };

            g_oblong = new Oblong(
                new Point(5, 5), startUpResolution.width - 5, startUpResolution.height - 5,
                Color.HotPink, GraphicsDevice);

            Rectangle rectFromOblong = (Rectangle)g_oblong;

            Textures.tex_McSelector = Content.Load<Texture2D>(@"Textures\peach_selector_animated.png");
           
            g_board = new Board(new Point(40, 30), mainTile);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            g_spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load static game content
            Textures.tex_Dummy = new Texture2D(GraphicsDevice, 1, 1);
            Textures.tex_SplashBg = Content.Load<Texture2D>(@"Textures\EUMD_LOGO.png");
            Textures.tex_HexSelector = Content.Load<Texture2D>(@"Textures\selectedHex.png");
            Textures.tex_MainMenuTitle = Content.Load<Texture2D>(@"Textures\BlockBustersCrackingTitle.png");
            Textures.tex_MainMenuBg = Content.Load<Texture2D>(@"Textures\blockbustersMainMenuBG.png");
            Textures.tex_InGameBg = Content.Load<Texture2D>(@"Textures\dark-polygonal-objects.jpg");
            Textures.tex_InfoContainer = Content.Load<Texture2D>(@"Textures\infoContainergfx.png");
            Textures.tex_qaContainer = Content.Load<Texture2D>(@"Textures\question_contain.png");
            Textures.tex_MainMenuSpinner = Content.Load<Texture2D>(@"Textures\purpleSpinParticle.png");
            Textures.tex_DBG_GPIC1 = Content.Load<Texture2D>(@"Textures\gpicC4PO.png");
            Textures.tex_DBG_GPIC2 = Content.Load<Texture2D>(@"Textures\gpicKENZI.png");
            Fonts.font_BoardLetters = Content.Load<SpriteFont>(@"blockbustersBoardFontQuartz38");
            Fonts.font_MainMenu = Content.Load<SpriteFont>(@"blockbustersMenuFont_bold24");
            Sounds.sfx_Splash = Content.Load<SoundEffect>(@"Sounds\EUMD_SPLASH");
            Sounds.sfx_MenuSweep = Content.Load<SoundEffect>(@"Sounds\menuSweep");
            Sounds.sfx_MenuTrack = Content.Load<SoundEffect>(@"Sounds\blockbustersAmbient");
            Sounds.sfx_Correct = Content.Load<SoundEffect>(@"Sounds\correct");
            Sounds.sfx_Incorrect = Content.Load<SoundEffect>(@"Sounds\incorrect");

            // Init Fading Splash
            g_fader = new Fader(Textures.tex_SplashBg, Point.Zero,
                (int)g_baseScreenSize.X, (int)g_baseScreenSize.Y, 
                FadeSequence.In_Out, 1f, 2f, 2f, 1.25f, 1.25f);
            
            // Init Main Menu
            string pg = "Play Game", opt = "Options", e = "Exit";
            g_mainMenu = new MainMenu(GraphicsDevice, Textures.tex_MainMenuBg).
                initAnimations(new Animated(Textures.tex_MainMenuTitle, 4, 0,
                    new Vector2((0.5f * g_baseScreenSize.X), 200f))).
                initSelectives(Color.FromNonPremultiplied(73, 181, 254, 255),
                new Selective {
                    Text = pg,
                    Colour = Color.White,
                    Font = Fonts.font_MainMenu,
                    Selected = false,
                    Hover = false,
                    Container = new Rectangle(-320, 320, (int)Fonts.font_MainMenu.MeasureString(pg).X,
                        (int)Fonts.font_MainMenu.MeasureString(pg).Y)
                },
                new Selective {
                    Text = opt,
                    Colour = Color.White,
                    Font = Fonts.font_MainMenu,
                    Selected = false,
                    Hover = false,
                    Container = new Rectangle(-320, 420, (int)Fonts.font_MainMenu.MeasureString(opt).X,
                        (int)Fonts.font_MainMenu.MeasureString(opt).Y)
                },
                new Selective {
                    Text = e,
                    Colour = Color.White,
                    Font = Fonts.font_MainMenu,
                    Selected = false,
                    Hover = false,
                    Container = new Rectangle(-320, 520, (int)Fonts.font_MainMenu.MeasureString(e).X,
                        (int)Fonts.font_MainMenu.MeasureString(e).Y)
                });

            // Init Hex Selector
            Point startHexCentre = g_board.BoardHexagons[7].destination.Center;
            g_hexSelector = new HexSelector(
                g_board,
                Textures.tex_HexSelector, 4, 0,
                new Vector2((float)startHexCentre.X, (float)startHexCentre.Y));
            g_hexSelector.FramesPerSecond = 30.0;
            g_hexSelector.Visibility = true;

            /***** DEBUG *****/
            g_players = new Player[2];
            g_players[0] = new Human("1UP", Textures.tex_DBG_GPIC1) { 
                Position = new Vector2(40f, 625f), ReverseLayout = false, Score = 0
            };
            g_players[1] = new Human("2UP", Textures.tex_DBG_GPIC2) {
                Position = new Vector2(1240f, 625f), ReverseLayout = true, Score = 0
            };

            g_board.Players = g_players;

            g_board.Players[0].TurnSpec = TurnSpecifier.Engaging;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of timing values.
        /// </param>
        protected override void Update(GameTime gameTime) {
            g_inputManager.updateInputDevices(PlayerIndex.One);
            
            switch (StateManager.gameState) { 
                case GameState.Splash:
                    g_fader.update(gameTime);
                    if (!g_bIsIntroStarted && g_fader.CurrentState.HasFlag(FadeState.InFade)) {
                        Sounds.sfx_Splash.Play(0.375f, 0f, 0f);
                        g_bIsIntroStarted = true;
                    }
                    if (g_fader.CurrentState == FadeState.Completed) {
                        StateManager.gameState = GameState.Main_Menu;
                        Textures.tex_SplashBg.Dispose();
                        Sounds.sfx_Splash.Dispose();
                    }
                    break;
                case GameState.Main_Menu:
                    g_mainMenu.update(gameTime, g_inputManager);
                    break;
                case GameState.Game_Running:
                    g_board.update(gameTime, g_inputManager);

                    if (!g_board.Selector.Visibility)
                        g_hexSelector.update(gameTime, g_inputManager);

                    g_board.ChosenHexIndex = g_hexSelector.ChosenHexIndex;
                    break;
                case GameState.Game_Paused:
                    break;
                case GameState.Credits:
                    break;
                case GameState.Exit:
                    Exit();
                    break;
                default:
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            // Resizes any graphics accordingly
            float hzScaling = (float)GraphicsDevice.PresentationParameters.BackBufferWidth / g_baseScreenSize.X;
            float vtScaling = (float)GraphicsDevice.PresentationParameters.BackBufferHeight / g_baseScreenSize.Y;
            Matrix globalScaler = Matrix.CreateScale(new Vector3(hzScaling, vtScaling, 1));

            g_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, globalScaler);

            switch (StateManager.gameState) {
                case GameState.Splash:
                    g_fader.draw(g_spriteBatch);
                    break;
                case GameState.Main_Menu:
                    g_oblong.draw(gameTime);
                    g_mainMenu.draw(g_spriteBatch);
                    break;
                case GameState.Game_Running:
                    g_board.draw(g_spriteBatch);
                    
                    if (!g_board.Selector.Visibility)
                        g_hexSelector.draw(g_spriteBatch);

                    break;
                case GameState.Game_Paused:
                    break;
                case GameState.Credits:
                    break;
                case GameState.Exit:
                    break;
                default:
                    break;
            }

            g_spriteBatch.End();
             
            base.Draw(gameTime);
        }

        #endregion
    }

    #endregion
}
