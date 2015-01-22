#region Prerequisites

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace BlockBusters.Graphics {

    #region Objects

    /// <summary>
    /// Class that handles simple sprite animations.
    /// </summary>
    public class Animated {

        #region Constructors

        /// <summary>
        /// Constructs an instance of Animated.
        /// </summary>
        /// <param name="spriteSheet">
        /// Represents the texture that the spritesheet is assigned to.
        /// </param>
        /// <param name="totalFrames">
        /// The total number of frames in the animation.
        /// </param>
        /// <param name="startingFrame">
        /// The frame to start the animation from.
        /// </param>
        /// <param name="startPosition">
        /// The position of the animation in the game.
        /// </param>
        public Animated(Texture2D spriteSheet, int totalFrames, int startingFrame, Vector2 startPosition) {
            m_spriteSheet = spriteSheet;
            m_totalFrames = totalFrames;
            m_currentFrame = startingFrame;
            m_position = startPosition;
            FramesPerSecond = 60.0;
            TotalRows = 1;
            CurrentRow = 0;
        }

        #endregion

        #region Declarations

        protected Texture2D m_spriteSheet;
        protected Rectangle m_srcRect, m_destination;
        protected Vector2 m_position;
        protected int m_totalFrames;
        protected int m_currentFrame;
        protected double m_currentTime;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a texture that represents the sprite sheet.
        /// </summary>
        public Texture2D SpriteSheet {
            get { return m_spriteSheet; }
        }

        /// <summary>
        /// Gets the rectangle that represents a section of the sprite sheet.
        /// </summary>
        public Rectangle SourceRectangle {
            get { return m_srcRect; }
        }

        /// <summary>
        /// Gets the rectangle that represents an area on the screen
        /// where the animated sprite will be rendered.
        /// </summary>
        public Rectangle DestinationRectangle {
            get { return m_destination; }
        }

        /// <summary>
        /// Gets a vector representing the current centre position of 
        /// the animation.
        /// </summary>
        public Vector2 Position {
            get { return m_position; }
            set { m_position = value; }
        }

        /// <summary>
        /// Gets and sets an integer that represents the total number of 
        /// frames in the animation.
        /// </summary>
        public int TotalFrames {
            get { return m_totalFrames; }
            set { m_totalFrames = value; }
        }

        /// <summary>
        /// Gets and sets an integer that represents the current row in
        /// the animation.
        /// </summary>
        public int CurrentRow { get; set; }

        /// <summary>
        /// Gets and sets an integer that represents the total number
        /// of rows in the animation.
        /// </summary>
        public int TotalRows { get; set; }

        /// <summary>
        /// Gets and sets an integer that represents the current frame in
        /// the animation. 
        /// </summary>
        public int CurrentFrame {
            get { return m_currentFrame; }
            set { m_currentFrame = value; }
        }

        /// <summary>
        /// Gets a double representing the current time.
        /// </summary>
        public double CurrentTime {
            get { return m_currentTime; }
        }

        /// <summary>
        /// Gets and sets Frames Per Second.
        /// </summary>
        public double FramesPerSecond { get; set; }

        /// <summary>
        /// Gets and sets the Visibility of this object.
        /// </summary>
        public bool Visibility { get; set; }

        #endregion

        #region Overloads

        /// <summary>
        /// Operator overload that allows the current frame to be positively
        /// iterated by the instance.
        /// </summary>
        /// <param name="animated">
        /// Parameter represents an instance of Animated.
        /// </param>
        /// <returns>
        /// An instance of Animated.
        /// </returns>
        public static Animated operator ++(Animated animated) {
            ++animated.CurrentFrame;
            return animated;
        }

        /// <summary>
        /// Operator overload that allows the current frame to be negatively
        /// iterated by the instance.
        /// </summary>
        /// <param name="animated">
        /// Parameter represents an instance of Animated.
        /// </param>
        /// <returns>
        /// An instance of Animated.
        /// </returns>
        public static Animated operator --(Animated animated) {
            --animated.CurrentFrame;
            return animated;
        }

        #endregion

        #region Functions

        /// <summary>
        /// Updates the animation, changing frames.
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of timing values.
        /// </param>
        public virtual void updateAnimation(GameTime gameTime) {
            m_currentTime += gameTime.ElapsedGameTime.TotalMilliseconds;

            int width = (m_spriteSheet.Width / m_totalFrames);
            int height = (m_spriteSheet.Height / TotalRows);

            m_srcRect = new Rectangle((m_currentFrame * width), (CurrentRow * height), width, height);

            if (m_currentTime >= (1000.0 / FramesPerSecond)) {
                ++m_currentFrame;
                m_currentTime = 0.0;
            }

            if (m_currentFrame >= m_totalFrames) {
                m_currentFrame = 0;
                if (TotalRows > 1)
                    ++CurrentRow;
            }

            if (CurrentRow >= TotalRows) {
                CurrentRow = 0;
                m_currentTime = 0;
            }

            m_destination = new Rectangle(
                    (int)((int)m_position.X - (0.5 * (m_spriteSheet.Width / m_totalFrames))),
                    (int)((int)m_position.Y - (0.5 * (m_spriteSheet.Height / TotalRows))),
                    width, height);
        }

        /// <summary>
        /// Renders the animation to the screen.
        /// </summary>
        /// <param name="spriteBatch">
        /// Parameter represents the spritebatch, used
        /// for drawing textures and alike.
        /// </param>
        public virtual void draw(SpriteBatch spriteBatch) {
            if (Visibility)
                spriteBatch.Draw(m_spriteSheet, m_destination, m_srcRect, Color.White);
        }

        #endregion
    }

    #endregion
}
