#region Prerequisites

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using BlockBusters.Main;
using BlockBusters.Sys;
using BlockBusters.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BlockBusters.Players;
using EUMD_CS.Graphics;

#endregion

namespace BlockBusters.Graphics {

    #region Enumerators

    /// <summary>
    /// Enumerator that tracks the current state of the board. 
    /// </summary>
    public enum BoardState { 
        ScramblingColours,
        ScramblingLetters,
        Standard,
        Resetting
    }

    #endregion

    #region Structures

    /// <summary>
    /// A structure that defines a single hexagon on the board.
    /// </summary>
    public struct BoardHexagon {
        public Texture2D hexagon;
        public Rectangle source, destination;
        public int id;
    }

    #endregion

    #region Objects

    /// <summary>
    /// Class represents the main game board.
    /// </summary>
    public class Board {

        #region Constructors

        /// <summary>
        /// Creates an instance of Board.
        /// </summary>
        /// <param name="bounds">
        /// A rectangle to bind the board to.
        /// </param>
        /// <param name="tile">
        /// Represents a tile to be used to decorate the board.
        /// </param>
        public Board(Point baseOffset, Tile tile) {
            m_baseOffset = baseOffset;
            m_tile = tile;

            boardLetters = new char[20];
            if (readBoardMap(@"Content\Textures\board.txt")) {
                initBoard();
                boardLetters = generateLetters();
            }
            
            m_scrambleTimer = m_scramblePerTimer = 0.0;
            m_boardState = BoardState.ScramblingColours;
            m_initPlayables = true;
            m_playableIndices = new List<int>();
            m_hexMapper = new Dictionary<Vector2, Color>();
            m_resultTimer = .0f;
            mb_playResultSound = true;
            mb_lastHex = mb_bonusActive = mb_edge1 = mb_edge2 = false;
            m_pindex = m_qChances = 0;

            // Create Rectangles for the 4 choices
            m_mcRects = new Rectangle[4];
            int y = 180;
            for (int r = 0; r < m_mcRects.Length; ++r) {
                m_mcRects[r] = new Rectangle(660, y, 590, 60);
                y += 80;
            }

            // Create a selector for the multi choice questions
            m_mcSelector = new Selector(Textures.tex_McSelector, 8, 0,
                new Vector2((float)m_mcRects[0].Center.X, (float)m_mcRects[0].Center.Y));
        }

        #endregion

        #region Declarations

        private Point m_baseOffset;
        private Tile m_tile;
        private List<Row> m_rows;
        private List<BoardHexagon> m_hexagons;
        private List<int> m_playableIndices;
        private char[] boardLetters;
        private double m_scrambleTimer, m_scramblePerTimer;
        private BoardState m_boardState;
        private QA m_currentQA;
        private Rectangle[] m_mcRects;
        private Selector m_mcSelector;
        private bool mb_playerCorrect, mb_displayResult, mb_playResultSound,
            mb_lastHex, m_initPlayables, mb_bonusActive;
        private float m_resultTimer;
        private int m_pindex, m_qChances;
        private Dictionary<Vector2, Color> m_hexMapper;
        private bool mb_edge1, mb_edge2;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of rows.
        /// </summary>
        public List<Row> Rows {
            get { return m_rows; }
        }

        /// <summary>
        /// Gets the array of board letters.
        /// </summary>
        public char[] BoardLetters {
            get { return boardLetters; }
        }

        /// <summary>
        /// Gets the list of Board Hexagons.
        /// </summary>
        public List<BoardHexagon> BoardHexagons {
            get { return m_hexagons; }
        }

        /// <summary>
        /// Gets a Tile.
        /// </summary>
        public Tile HexTile {
            get { return m_tile; }
        }

        /// <summary>
        /// Gets and sets the Board State.
        /// </summary>
        public BoardState CurrentBoardState {
            get { return m_boardState; }
            set { m_boardState = value; }
        }

        /// <summary>
        /// Gets and sets the current question and answer.
        /// </summary>
        public QA CurrentQA {
            get { return m_currentQA; }
            set { m_currentQA = value; }
        }

        /// <summary>
        /// Sets the boolean that dictates whether we 
        /// should render the question and answer.
        /// </summary>
        public bool RenderQA {
            get; set;
        }

        /// <summary>
        /// Gets the multi-choice selector.
        /// </summary>
        public Selector Selector {
            get { return m_mcSelector; }
        }

        /// <summary>
        /// Gets and sets the players for the board.
        /// </summary>
        public Player[] Players { get; set; }

        /// <summary>
        /// Gets and sets the chosen hexagon index.
        /// </summary>
        public int ChosenHexIndex { get; set; }

        /***** DEBUG *****/

        public double ScrambleTimer {
            get { return m_scrambleTimer; }
            set { m_scrambleTimer = value; }
        }

        public double ScramblePerTimer {
            get { return m_scramblePerTimer; }
            set { m_scramblePerTimer = value; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Reads the board mapping from a text file and aids
        /// tile generation for the board.
        /// </summary>
        /// <param name="path">
        /// A string representing the path to the text file.
        /// </param>
        /// <returns>
        /// True, if the read is successful.
        /// False otherwise.
        /// </returns>
        private bool readBoardMap(string path) {
            string[] lines = null;
            uint msgID = MessageBoxButtonOutput.IDRETRY;

            while (msgID == MessageBoxButtonOutput.IDRETRY) {
                try {
                    lines = File.ReadAllLines(path);
                    msgID = MessageBoxButtonOutput.IDOK;
                } catch (IOException) {
                  #if WINDOWS
                    msgID = _WINAPI.MessageBox(
                        new IntPtr(0), "Failed to locate board.txt...", "Error!",
                        MessageBoxType.MB_ABORTRETRYIGNORE);      
                  #endif
                    if (msgID == MessageBoxButtonOutput.IDIGNORE)
                        break;
                    if (msgID == MessageBoxButtonOutput.IDABORT) {
                        StateManager.gameState = GameState.Exit;
                        break;
                    } 
                }
            }

            if (msgID == MessageBoxButtonOutput.IDOK) {
                m_rows = new List<Row>();
                foreach (string line in lines) {
                    Row thisRow = new Row();
                    string[] ids = Regex.Split(line, @"\D+");
                    foreach (string val in ids) {
                        if (!string.IsNullOrEmpty(val)) {
                            thisRow.Columns.Add((Cell)int.Parse(val));
                        }
                    }
                    m_rows.Add(thisRow);
                }
            }

            return (msgID == MessageBoxButtonOutput.IDOK);
        }

        /// <summary>
        /// Initiates all the positions of the hexagons on the board.
        /// </summary>
        private void initBoard() {
            m_hexagons = new List<BoardHexagon>();
            
            for (int y = 0; y < m_rows.Count; ++y) {
                int rowOffset = (y % 2 == 1 ? 0 : m_tile.evenRow_offsetX);
                for (int x = 0; x < m_rows[y].Columns.Count; ++x) {
                    foreach (int tileID in m_rows[y].Columns[x].BaseTiles) {
                        Rectangle _destination;
                        int _x;

                        if (y == (m_rows.Count - 1))
                            _x = (x * m_tile.stepX) + rowOffset + m_tile.stepX + m_baseOffset.X;
                        else
                            _x = (x * m_tile.stepX) + rowOffset + m_baseOffset.X;

                        _destination = new Rectangle(
                            _x, (y * m_tile.stepY) + m_baseOffset.Y,
                            m_tile.width, m_tile.height);

                        m_hexagons.Add(
                            new BoardHexagon {
                                hexagon = m_tile.spriteSheet,
                                destination = _destination,
                                source = m_tile.getSrcRect(tileID),
                                id = tileID
                            });
                    }
                }
            }
            m_boardState = BoardState.Standard;
        }

        /// <summary>
        /// Scrambles the colours of the playable hexagons on the board.
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of timing values.
        /// </param>
        private void colourScramble(GameTime gameTime) {
            double duration = 2.0;

            if (m_initPlayables) {
                for (int i = 0; i < m_hexagons.Count; ++i) {
                    if (m_hexagons[i].id.Equals(1))
                        m_playableIndices.Add(i);
                }
                m_initPlayables = false;
            }

            if (m_scrambleTimer < duration) {
                Random r = new Random();
                for (int i = 0; i < m_playableIndices.Count; ++i) {
                    BoardHexagon tmp = m_hexagons[m_playableIndices[i]];
                    tmp.source = m_tile.getSrcRect(r.Next(0, 3));
                    m_hexagons[m_playableIndices[i]] = tmp;
                }
            } else 
                m_boardState = BoardState.Resetting;
        }

        /// <summary>
        /// Checks whether the game has been won.
        /// </summary>
        /// <param name="playerColor">
        /// Indicates which player we are going to check,
        /// using their color.
        /// </param>
        /// <returns>
        /// True if the player's color has crossed the board,
        /// being linked all the way across.
        /// </returns>
        private bool isBlockbusters(Vector2 startPosition, Color playerColor) {
            // First let's see if we can find a k/v pair that matches the arguments
            var temp = m_hexMapper.Where(h => h.Key == startPosition)
                .Select(h => new { Key = h.Key, Value = h.Value })
                .FirstOrDefault();

            if (temp == null || temp.Value != playerColor) return false;

            // Set up the playable board bounds (helps us define the edges)
            float topBound = (float)m_hexagons[7].destination.Center.Y;
            float leftBound = (float)m_hexagons[7].destination.Center.X;
            float rightBound = (float)m_hexagons[9].destination.Center.X;
            float bottomBound = (float)m_hexagons[33].destination.Center.Y;

            // Check if we have any of the edges (Blue - left to right, White - top to bottom)
            if ((temp.Key.X == leftBound && playerColor == Color.Blue) ||
                (temp.Key.Y == topBound && playerColor == Color.White))
                mb_edge1 = true;
            if ((temp.Key.X == rightBound && playerColor == Color.Blue) ||
                (temp.Key.Y == bottomBound && playerColor == Color.White))
                mb_edge2 = true;

            // Get a random number (6 possibilities)
            Vector2 next = Vector2.Zero;
            Random r = new Random();
            int nri = r.Next(0, 6);

            // Grab one of the surrounding hexagons using the random number from above
            switch (nri) { 
                case 0: next.X = temp.Key.X; next.Y = (temp.Key.Y - 82f); break;
                case 1: next.X = (temp.Key.X + 80f); next.Y = (temp.Key.Y - 43f); break;
                case 2: next.X = (temp.Key.X + 80f); next.Y = (temp.Key.Y + 43f); break;
                case 3: next.X = temp.Key.X; next.Y = (temp.Key.Y + 82f); break;
                case 4: next.X = (temp.Key.X - 80f); next.Y = (temp.Key.Y + 43f); break;
                case 5: next.X = (temp.Key.X - 80f); next.Y = (temp.Key.Y - 43f); break;
                default: break;
            }

            // Run recursively until we have both edges
            return (mb_edge1 && mb_edge2) ? true : isBlockbusters(next, playerColor);
        }

        /// <summary>
        /// Updates and performs the logic of the board.
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of timing values.
        /// </param>
        public void update(GameTime gameTime, InputManager inputManager) {
            switch (m_boardState) { 
                case BoardState.ScramblingColours:
                    double scramblePerSec = .25;
                    m_scramblePerTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    m_scrambleTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (m_scramblePerTimer > scramblePerSec) {
                        colourScramble(gameTime);
                        m_scramblePerTimer = 0.0;
                    }
                    break;
                case BoardState.ScramblingLetters:
                    break;
                case BoardState.Resetting:
                    initBoard();
                    refreshLetters();
                    break;
                case BoardState.Standard:
                    Players[m_pindex].TurnSpec = TurnSpecifier.Engaging;

                    if (m_mcSelector.Visibility) {
                        if (m_mcSelector.Mutex.Equals(Selector.SelectorMutex.Unlocked)) {
                            if (inputManager.isKeyTapped(Keys.Down)) {
                                if (m_mcSelector.Position.Y < m_mcRects[3].Center.Y) {
                                    m_mcSelector.move(Selector.Direction.MoveDown,
                                        (float)m_mcRects[1].Center.Y - (float)m_mcRects[0].Center.Y);
                                }
                            } else if (inputManager.isKeyTapped(Keys.Up)) {
                                if (m_mcSelector.Position.Y > m_mcRects[0].Center.Y) {
                                    m_mcSelector.move(Selector.Direction.MoveUp,
                                        (float)m_mcRects[1].Center.Y - (float)m_mcRects[0].Center.Y);
                                }
                            }

                            if (inputManager.isKeyTapped(Keys.Enter)) {
                                if ((int)m_mcSelector.Position.Y == m_mcRects[0].Center.Y)
                                    mb_playerCorrect = (m_currentQA.answer == 'A');
                                else if ((int)m_mcSelector.Position.Y == m_mcRects[1].Center.Y)
                                    mb_playerCorrect = (m_currentQA.answer == 'B');
                                else if ((int)m_mcSelector.Position.Y == m_mcRects[2].Center.Y)
                                    mb_playerCorrect = (m_currentQA.answer == 'C');
                                else if ((int)m_mcSelector.Position.Y == m_mcRects[3].Center.Y)
                                    mb_playerCorrect = (m_currentQA.answer == 'D');

                                mb_displayResult = true;
                                mb_playResultSound = true;
                            }

                            m_mcSelector.Mutex = Selector.SelectorMutex.Locked;
                        }

                        if (mb_displayResult) { 
                            // Start timer
                            m_resultTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if (m_resultTimer >= 1.5f) {
                                m_resultTimer = .0f;
                                mb_displayResult = false;
                                m_mcSelector.Position = new Vector2(
                                    (float)m_mcRects[0].Center.X, (float)m_mcRects[0].Center.Y);

                                if (m_qChances == 0 || m_qChances == 2) {
                                    m_mcSelector.Visibility = false;
                                    RenderQA = false;
                                }
                            }

                            if (mb_playResultSound) {
                                int color_index = (m_pindex == 0) ? m_pindex : 2;
                                int bl_index = 0, h_index = 0;

                                foreach (var hex in m_hexagons) {
                                    if (hex.id == 1) {
                                        if (bl_index == ChosenHexIndex) {
                                            h_index = m_hexagons.IndexOf(hex);
                                            break;
                                        }
                                        ++bl_index;
                                    }
                                }
                                
                                if (mb_playerCorrect) {
                                    Sounds.sfx_Correct.Play(.35f, 1f, 0f);
                                    Players[m_pindex].Correct = true;
                                    Players[m_pindex].PWinType = mb_lastHex ? WinType.Round : WinType.Single;
                                    Players[m_pindex].update();

                                    if (mb_bonusActive) {
                                        Players[m_pindex].Score += 25;
                                        mb_bonusActive = false;
                                    }

                                    BoardHexagon tmp = m_hexagons[h_index];
                                    tmp.source = m_tile.getSrcRect(color_index);
                                    m_hexagons[h_index] = tmp;
                                    boardLetters[ChosenHexIndex] = ' ';
                                    m_qChances = 0;

                                    Color changed = (color_index == 0) ? Color.White : Color.Blue;
                                    Vector2 chosenPos = new Vector2(
                                        (float)tmp.destination.Center.X, (float)tmp.destination.Center.Y);
                                    m_hexMapper.Add(chosenPos, changed);

                                    // @TODO: Check for game win....
                                    if (isBlockbusters(chosenPos, changed)) { 
                                        // End game for now, until we develop summat more...
                                        StateManager.gameState = GameState.Exit;
                                    }

                                } else {
                                    ++m_qChances;
               
                                    Sounds.sfx_Incorrect.Play(.35f, 1f, 0f);
                                    Players[m_pindex].TurnSpec = TurnSpecifier.Waiting;
                                    m_pindex = (m_pindex == (Players.Length - 1)) ? 0 : m_pindex + 1;

                                    if (m_qChances == 1) {
                                        mb_bonusActive = true;
                                    } else if (m_qChances == 2) {
                                        m_qChances = 0;
                                        BoardHexagon tmp = m_hexagons[h_index];
                                        tmp.source = m_tile.getSrcRect(3);
                                        m_hexagons[h_index] = tmp;
                                        boardLetters[ChosenHexIndex] = ' ';
                                        mb_bonusActive = false;
                                    }
                                }

                                mb_playResultSound = false;
                            }
                        }

                        m_mcSelector.update(gameTime);
                    }
                    break;
            }
        }

        /// <summary>
        /// Draw the game board.
        /// </summary>
        /// <param name="spriteBatch">
        /// Parameter represents the spritebatch, used
        /// for drawing textures and alike.
        /// </param>
        public void draw(SpriteBatch spriteBatch) {
            int i = 0;
            Vector2 charPosition = Vector2.Zero;
            
            // Draw background
            spriteBatch.Draw(Textures.tex_InGameBg, new Rectangle(
                0, 0, Textures.tex_InGameBg.Width, Textures.tex_InGameBg.Height), Color.White);

            // Draw Game Board
            foreach (BoardHexagon hex in m_hexagons) {
                spriteBatch.Draw(hex.hexagon, hex.destination, hex.source, Color.White);
                if (hex.id == 1 && m_boardState.HasFlag(BoardState.Standard)) {
                    /* Need to bump the letter 'I' position to the right a little more
                     * with it not being as wide as the other letters. */
                    if (boardLetters[i].Equals('I')) {
                        charPosition = new Vector2(
                            hex.destination.X + (0.5f * (hex.hexagon.Width / 4)) - 5, 
                            hex.destination.Y + (0.5f * (hex.hexagon.Height / 4)) - 6f);
                    } else if (boardLetters[i].Equals('M') || boardLetters[i].Equals('W')) {
                        charPosition = new Vector2(
                            hex.destination.X + (0.5f * (hex.hexagon.Width / 4)) - 22.5f,
                            hex.destination.Y + (0.5f * (hex.hexagon.Height / 4)) - 6f);
                    } else if (boardLetters[i].Equals('T')) {
                        charPosition = new Vector2(
                            hex.destination.X + (0.5f * (hex.hexagon.Width / 4)) - 18f,
                            hex.destination.Y + (0.5f * (hex.hexagon.Height / 4)) - 6f);
                    } else {
                        charPosition = new Vector2(
                            hex.destination.X + (0.5f * (hex.hexagon.Width / 4)) - 12.5f,
                            hex.destination.Y + (0.5f * (hex.hexagon.Height / 4)) - 6f);
                    }
                    spriteBatch.DrawString(Fonts.font_BoardLetters, boardLetters[i].ToString(), 
                        charPosition, Color.Black);
                    ++i;
                }
            }

            // Draw Question and Answer container
            spriteBatch.Draw(Textures.tex_qaContainer, new Rectangle(
                1280 - 5 - Textures.tex_qaContainer.Width, 25,
                Textures.tex_qaContainer.Width, Textures.tex_qaContainer.Height), Color.White);

            /* Draw the question and answer within the above container */

            if (RenderQA) {
                m_mcSelector.Visibility = true;

                // We need to figure out how many characters we can fit within the container
                float charWidth = Fonts.font_MainMenu.MeasureString("A").X;
                int actualSpacial = Textures.tex_qaContainer.Width - 10;
                int lenQ = m_currentQA.question.Length;

                // How many characters will we get on?
                int maxChars = (int)Math.Floor((float)actualSpacial / charWidth);

                // Break the question down into a list of strings if needs be
                List<string> seperatedQuestion = new List<string>();
                string temp = string.Empty;
                int qindex = 0, round = 1;

                while (qindex < lenQ) {
                    int spos = m_currentQA.question.IndexOf(" ", qindex);

                    // Final word
                    if (spos == -1)
                        spos = lenQ - 1;

                    if (spos < maxChars * round) {
                        while (qindex <= spos) {
                            temp += m_currentQA.question[qindex];
                            ++qindex;
                        }
                        if (qindex == lenQ) {
                            seperatedQuestion.Add(temp);
                            temp = string.Empty;
                        }
                    } else {
                        seperatedQuestion.Add(temp);
                        temp = string.Empty;

                        // Add on final part if any
                        if (qindex < spos) {
                            while (qindex <= spos) {
                                temp += m_currentQA.question[qindex];
                                ++qindex;
                            }
                            if (qindex == lenQ) {
                                seperatedQuestion.Add(temp);
                                temp = string.Empty;
                            }
                        }
                        ++round;
                    }
                }

                // Now let's print out the question
                Vector2 qpos = new Vector2(660, 50);
                foreach (string s in seperatedQuestion) {
                    spriteBatch.DrawString(Fonts.font_MainMenu, s, qpos, Color.White);
                    qpos.Y += 20.0f;
                }

                // Draw the rectangles
                Textures.tex_Dummy.SetData(new Color[] { Color.FromNonPremultiplied(75, 110, 150, 255) });
                foreach (Rectangle r in m_mcRects) {
                    spriteBatch.Draw(Textures.tex_Dummy, r, Color.Turquoise);
                }

                // Draw the multi choices
                spriteBatch.DrawString(Fonts.font_MainMenu, "A. " + m_currentQA.A, new Vector2(
                    (float)m_mcRects[0].X + 10f, (float)m_mcRects[0].Y + 10.0f), Color.Black);
                spriteBatch.DrawString(Fonts.font_MainMenu, "B. " + m_currentQA.B, new Vector2(
                    (float)m_mcRects[1].X + 10f, (float)m_mcRects[1].Y + 10.0f), Color.Black);
                spriteBatch.DrawString(Fonts.font_MainMenu, "C. " + m_currentQA.C, new Vector2(
                    (float)m_mcRects[2].X + 10f, (float)m_mcRects[2].Y + 10.0f), Color.Black);
                spriteBatch.DrawString(Fonts.font_MainMenu, "D. " + m_currentQA.D, new Vector2(
                    (float)m_mcRects[3].X + 10f, (float)m_mcRects[3].Y + 10.0f), Color.Black);

                // Draw the selector
                m_mcSelector.draw(spriteBatch);

                if (mb_displayResult) {
                    m_mcSelector.Mutex = Selector.SelectorMutex.Locked;
                    
                    if (mb_playerCorrect) {
                        spriteBatch.DrawString(Fonts.font_MainMenu, "Correct, well done!",
                            new Vector2(660.0f, (float)m_mcRects[3].Center.Y + 60.0f), Color.LimeGreen);
                    } else {
                        spriteBatch.DrawString(Fonts.font_MainMenu, "Incorrect!",
                            new Vector2(660.0f, (float)m_mcRects[3].Center.Y + 60.0f), Color.Red);
                    }
                }

            } else {
                m_mcSelector.Visibility = false;
            }

            // Draw Info Container
            spriteBatch.Draw(Textures.tex_InfoContainer, new Rectangle(
                5, 600, Textures.tex_InfoContainer.Width, Textures.tex_InfoContainer.Height), Color.White);

            for (uint p = 0; p < Players.Length; ++p)
                Players[p].draw(spriteBatch);

            // Draw the scores, starting with the centre hyphen
            string centreHyphen = "-";
            float szHyphen = Fonts.font_MainMenu.MeasureString(centreHyphen).X;
            spriteBatch.DrawString(Fonts.font_MainMenu, centreHyphen, new Vector2(
                ((float)Textures.tex_InfoContainer.Width / 2f) - szHyphen,
                (float)CommonResolutions.WXGA_H.height - ((float)Textures.tex_InfoContainer.Height / 2f) 
                - 50f), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(Fonts.font_MainMenu, Players[0].Score.ToString(),
                new Vector2(((float)Textures.tex_InfoContainer.Width / 2f) - 15f -
                Fonts.font_MainMenu.MeasureString(Players[0].Score.ToString()).X * 2f,
                (float)CommonResolutions.WXGA_H.height - ((float)Textures.tex_InfoContainer.Height / 2f)
                - 50f), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(Fonts.font_MainMenu, Players[1].Score.ToString(),
                new Vector2(((float)Textures.tex_InfoContainer.Width / 2f) + 15f,
                (float)CommonResolutions.WXGA_H.height - ((float)Textures.tex_InfoContainer.Height / 2f)
                - 50f), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Creates a unique array of letters for the board.
        /// </summary>
        /// <returns>
        /// An array of type char.
        /// </returns>
        private char[] generateLetters() {
            char[] letters = new char[20];
            List<char> tmp = new List<char>();
            Random r = new Random();

            for (int i = 0; i < letters.Length; ++i) {
                int n = r.Next('A', 'Z');
                /* We do not want either the character 'X' or 'Z',
                 * nor do we want any duplicates within the list of letters. */
                if (n != 'X' && n != 'Z' && !tmp.Exists(x => x == n))
                    tmp.Add((char)n);
                else
                    --i;
            }

            /* Copy the list to the fixed size array.
             * List is used initially in order to make
             * use of its Exists function. */
            letters = tmp.ToArray();
            return letters;
        }

        /// <summary>
        /// Fills up the array of letters with a fresh set.
        /// </summary>
        public void refreshLetters() {
            boardLetters = generateLetters();
        }

        #endregion
    }

    #endregion
}
