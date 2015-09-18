#region Prerequisites

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace BlockBusters.Graphics {

    #region Objects

    /// <summary>
    /// Class that defines a selector.
    /// The selector is an animated, hollow, rectangular object
    /// that is used to navigate the options within the user interface.
    /// </summary>
    public class Selector : Animated {

        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Selector() : base(null, 0, 0, Vector2.Zero) { ; }

        /// <summary>
        /// Creates an instance of a Selector.
        /// </summary>
        /// <param name="spritesheet">
        /// Represents the texture holding the animation.
        /// </param>
        /// <param name="nFrames">
        /// Indicates the number of frames in the animation.
        /// </param>
        /// <param name="startingFrame">
        /// The starting frame of the animation (set to zero for the beginning).
        /// </param>
        /// <param name="startPosition">
        /// Indicates the location within the viewport where the selector will be placed.
        /// NOTE: The animation class uses the centre of the frame for placement!
        /// </param>
        public Selector(Texture2D spritesheet, int nFrames, int startingFrame, Vector2 startPosition) :
            base(spritesheet, nFrames, startingFrame, startPosition) { m_lockTimer = .0f; }

        #endregion

        #region Enumerators

        /// <summary>
        /// Tracks the movement of the selector
        /// </summary>
        public enum Direction {
            MoveUp, MoveDown, MoveLeft, MoveRight, NoMove
        }

        /// <summary>
        /// A small mutex that tracks the lock status of the selector.
        /// This attribute dictates whether the selecotr can move or not.
        /// </summary>
        public enum SelectorMutex {
            Unlocked, Locked
        }

        #endregion

        #region Members

        private Direction m_direction, m_prevDirection;
        private SelectorMutex m_mutex;
        private float m_lockTimer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the direction of the next movement.
        /// </summary>
        public Direction SelectorDirection {
            get { return m_direction; }
            set { m_direction = value; }
        }

        /// <summary>
        /// Gets the previous direction.
        /// </summary>
        public Direction PreviousDirection {
            get { return m_prevDirection; }
        }

        /// <summary>
        /// Gets and sets the mutex.
        /// </summary>
        public SelectorMutex Mutex {
            get { return m_mutex; }
            set { m_mutex = value; }
        }

        /// <summary>
        /// Public boolean that indicates that our selector has indeed
        /// selected something.
        /// </summary>
        public bool Selected { get; set; }

        #endregion

        #region Functions

        /// <summary>
        /// Updates the logic and animation of the selector.
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of the games timing values.
        /// </param>
        public void update(GameTime gameTime) {
            updateAnimation(gameTime);

            // Check to see if the mutex is locked, if so start the timer
            if (m_mutex == SelectorMutex.Locked) {
                m_lockTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                // When we have reached 1/3 of a second, unlock the mutex
                if (m_lockTimer >= 3.333f) {
                    m_lockTimer = .0f;
                    m_mutex = SelectorMutex.Unlocked;
                }
            }
        }

        /// <summary>
        /// Moves the selector in the specified direction
        /// by the specified amount (simples!).
        /// NOTE: Update must be called afterwards to complete
        /// the visual affect of this function.
        /// </summary>
        /// <param name="direction">
        /// Indicates the direction of movement.
        /// </param>
        /// <param name="amount">
        /// Indicates how far the selector should move in one frame.
        /// </param>
        public void move(Direction direction, float amount) {
            if (m_mutex == SelectorMutex.Unlocked) {
                switch (direction) {
                    case Direction.MoveUp:
                        m_position.Y -= amount;
                        break;
                    case Direction.MoveDown:
                        m_position.Y += amount;
                        break;
                    case Direction.MoveLeft:
                        m_position.X -= amount;
                        break;
                    case Direction.MoveRight:
                        m_position.X += amount;
                        break;
                    case Direction.NoMove:
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion
    }

    #endregion
}

// END OF FILE
