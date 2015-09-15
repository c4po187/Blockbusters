#region Prerequisites

using System.Collections.Generic;
using BlockBusters.Sys;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace BlockBusters.Graphics {

    /// <summary>
    /// Enumerator that assists in moving the HexSelector
    /// </summary>
    public enum MoveDirection { 
        Up,
        Down,
        LeftUp,
        LeftDown,
        RightUp,
        RightDown,
        Idle
    }

    #region Objects

    public class HexSelector : Animated {

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="board">
        /// Parameter represents the game boad.
        /// </param>
        /// <param name="spriteSheet">
        /// The texture includes this objects animation graphics.
        /// </param>
        /// <param name="nFrames">
        /// The number of frames in the graphical animation.
        /// </param>
        /// <param name="startingFrame">
        /// The frame to start the animation from.
        /// </param>
        /// <param name="startingPosition">
        /// The starting position of this object.
        /// </param>
        public HexSelector(Board board, Texture2D spriteSheet, 
            int nFrames, int startingFrame, Vector2 startingPosition) :
            base(spriteSheet, nFrames, startingFrame, startingPosition) {
            m_board = board;
            m_bisDelay = false;
            m_delayTimer = 0.0;
            m_chosenLetter = '.';
            CurrentRow = 0;
            initPlayables();
        }

        #endregion

        #region Declarations

        private List<BoardHexagon> m_playableHexagons;
        private Board m_board;
        private bool m_bisDelay;
        private double m_delayTimer;
        private char m_chosenLetter;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the chosen letter.
        /// </summary>
        public char ChosenLetter {
            get { return m_chosenLetter; }
            /***** DEBUG *****/
            set { m_chosenLetter = value; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Fills up a list that determines which hexagons on the board
        /// our selector can select.
        /// </summary>
        private void initPlayables() {
            m_playableHexagons = m_board.BoardHexagons.FindAll(x => x.id.Equals(1));
        }

        /// <summary>
        /// Gets the letter that the user selected.
        /// </summary>
        /// <returns>
        /// A char.
        /// </returns>
        public char getLetterFromCurrentHexagon() {
            Point pPos = new Point((int)m_position.X, (int)m_position.Y);
            return m_board.BoardLetters[m_playableHexagons.FindIndex(
                x => x.destination.Center.Equals(pPos))];
        }

        /// <summary>
        /// Function that calculates the direction of the selector based
        /// on user input.
        /// </summary>
        /// <param name="inputManager">
        /// Parameter represents the current input manager.
        /// </param>
        /// <returns>
        /// A MoveDirection.
        /// </returns>
        private MoveDirection getDirection(InputManager inputManager, float bottomBound) {
            if (inputManager.isKeyTapped(Keys.Up)) {
                m_bisDelay = false;
                return MoveDirection.Up;
            }
            else if (inputManager.isKeyTapped(Keys.Down)) {
                m_bisDelay = false;
                return MoveDirection.Down;
            }
            else if (inputManager.isKeyTapped(Keys.Left)) {
                m_bisDelay = false;
                return (m_position.Y >= bottomBound) ? MoveDirection.LeftUp : MoveDirection.LeftDown;
            }
            else if (inputManager.isKeyTapped(Keys.Right)) {
                m_bisDelay = false;
                return (m_position.Y >= bottomBound) ? MoveDirection.RightUp : MoveDirection.RightDown;
            }
            else if (inputManager.isLeftThumbstickFacingUp() || inputManager.isUpDPadTapped())
                return MoveDirection.Up;
            else if (inputManager.isLeftThumbstickFacingDown() || inputManager.isDownDPadTapped())
                return MoveDirection.Down;
            else if (inputManager.isLeftDPadTapped()) {
                return (m_position.Y >= bottomBound) ? MoveDirection.LeftUp : MoveDirection.LeftDown;
            }
            else if (inputManager.isRightDPadTapped()) {
                return (m_position.Y >= bottomBound) ? MoveDirection.RightUp : MoveDirection.RightDown;
            }
            else if (inputManager.isLeftThumbstickFacingLeft()) {
                return (inputManager.LeftThumbstickPosition.Value.Y <= 0) ? 
                    MoveDirection.LeftDown : MoveDirection.LeftUp;
            }
            else if (inputManager.isLeftThumbstickFacingRight()) {
                return (inputManager.LeftThumbstickPosition.Value.Y <= 0) ?
                    MoveDirection.RightDown : MoveDirection.RightUp;
            }
            else
                return MoveDirection.Idle;
        }

        /// <summary>
        /// Sets the selector to the position of the mouse.
        /// </summary>
        /// <param name="inputManager">
        /// Parameter represents the current input manager.
        /// </param>
        /// <returns>
        /// True if the mouse is in a playable hexagon.
        /// False otherwise.
        /// </returns>
        private bool relativeToMouse(InputManager inputManager) {
            bool isRelative = false;
            
            foreach (BoardHexagon hex in m_playableHexagons) {
                if (inputManager.MouseLocation.X > hex.destination.Left &&
                    inputManager.MouseLocation.X < hex.destination.Right &&
                    inputManager.MouseLocation.Y > hex.destination.Top &&
                    inputManager.MouseLocation.Y < hex.destination.Bottom) {
                    
                    m_position.X = (float)hex.destination.Center.X;
                    m_position.Y = (float)hex.destination.Center.Y;
                    isRelative = true;
                    
                    if (inputManager.isLeftMouseButtonTapped() && Visibility)
                        m_chosenLetter = getLetterFromCurrentHexagon();
                    if (inputManager.isRightMouseButtonTapped() && Visibility) {
                        m_board.CurrentBoardState = BoardState.ScramblingColours;
                        m_board.ScramblePerTimer = m_board.ScrambleTimer = 0.0;
                    }

                    break;
                }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
            }

            return isRelative;
        }

        /// <summary>
        /// Updates the logic of the Selector.
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of timing values.
        /// </param>
        /// <param name="inputManager">
        /// Parameter represents the current input manager.
        /// </param>
        public void update(GameTime gameTime, InputManager inputManager) {
            // Update the animation
            updateAnimation(gameTime);

            // Previous Position
            Vector2 prevPosition = new Vector2(m_position.X, m_position.Y);

            // Detect selection
            if ((inputManager.isKeyTapped(Keys.Enter) || inputManager.isATapped()) && Visibility)
                m_chosenLetter = getLetterFromCurrentHexagon();

            /***** DEBUG *****/
            if (inputManager.isKeyTapped(Keys.Escape) && Visibility)
                m_board.refreshLetters();
            if (inputManager.isKeyTapped(Keys.C) && Visibility) {
                m_board.CurrentBoardState = BoardState.ScramblingColours;
                m_board.ScramblePerTimer = m_board.ScrambleTimer = 0.0;
            }
                
            // Bound parameters
            float topBound = (float)m_board.BoardHexagons[7].destination.Center.Y;
            float leftBound = (float)m_board.BoardHexagons[7].destination.Center.X;
            float rightBound = (float)m_board.BoardHexagons[9].destination.Center.X;
            float bottomBound = (float)m_board.BoardHexagons[33].destination.Center.Y;

            // Check for Mouse position
            if (!relativeToMouse(inputManager)) {
                // Set movement parameters
                float hzMove = (float)m_board.HexTile.evenRow_offsetX;
                float vtMove = (float)m_board.HexTile.height + 4;
                MoveDirection moveDirection = getDirection(inputManager, bottomBound);

                // Move selector
                if (!m_bisDelay) {
                    Visibility = true;
                    switch (moveDirection) {
                        case MoveDirection.Up:
                            m_position.Y -= vtMove;
                            break;
                        case MoveDirection.Down:
                            m_position.Y += vtMove;
                            break;
                        case MoveDirection.LeftUp:
                            m_position += new Vector2(-hzMove, -(0.5f * vtMove));
                            break;
                        case MoveDirection.LeftDown:
                            m_position += new Vector2(-hzMove, (0.5f * vtMove));
                            break;
                        case MoveDirection.RightUp:
                            m_position += new Vector2(hzMove, -(0.5f * vtMove));
                            break;
                        case MoveDirection.RightDown:
                            m_position += new Vector2(hzMove, (0.5f * vtMove));
                            break;
                        case MoveDirection.Idle:
                            break;
                    }
                    m_bisDelay = true;
                }
                else {
                    // Create a slight delay before we can move again
                    m_delayTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (m_delayTimer >= 0.1325) {
                        m_delayTimer = 0.0;
                        m_bisDelay = false;
                    }
                }
            }

            // Check bounds and return to previous position if exceeded
            if (m_position.Y < topBound || m_position.Y > bottomBound ||
                m_position.X < leftBound || m_position.X > rightBound) {
                Visibility = false;
                m_position = prevPosition;
            }
        }

        #endregion
    }

    #endregion
}
