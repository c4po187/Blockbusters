#region Prerequisites

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace BlockBusters.Sys {

    #region Objects

    /// <summary>
    /// Class manages the games input devices.
    /// </summary>
    public class InputManager {

        #region Constructor

        /// <summary>
        /// Creates a new instance of InputManager.
        /// </summary>
        public InputManager() {
            m_padConnected = false;
        }

        #endregion

        #region Declarations

        /* Gamepad Connection Flag */
        private bool            m_padConnected;

        /* Mouse Fields */
        private int             m_mouseDeltaX,  m_mouseDeltaY,
                                m_prevScrVal,   m_curScrVal,    m_scrDelta;

        /***** DEBUG *****/
        private double          m_deltaDivisor;

        /* Input Device States */
        private KeyboardState   m_prevKB,       m_curKB;
        private MouseState      m_prevMouse,    m_curMouse;
        private GamePadState    m_prevPad,      m_curPad;

        #endregion

        #region Properties

        /// <summary>
        /// Returns a Point that represents the current mouse co-ordinates.
        /// </summary>
        public Point MouseLocation {
            get { return m_curMouse.Position; }
        }

        /// <summary>
        /// Returns a Point that represents the difference between the previous,
        /// and current mouse co-ordinates.
        /// </summary>
        public Point MouseDelta {
            get { return new Point(m_mouseDeltaX, m_mouseDeltaY); }
        }

        /// <summary>
        /// Returns an integer that represents the current value of the
        /// mouse scroll-wheel.
        /// </summary>
        public int MouseWheelVal {
            get { return m_curScrVal; }
        }

        /// <summary>
        /// Returns an integer that represents the current delta of the
        /// previous and current mouse scroll-wheel values.
        /// </summary>
        public int ScrollDelta {
            get { return m_scrDelta; }
        }

        /// <summary>
        /// Returns a float representing the current value of the left trigger.
        /// The value has the range of 0 - 1.0, depending on the depth of application.
        /// </summary>
        public float? LeftTriggerDepth {
            get {
                if (m_padConnected)
                    return m_curPad.Triggers.Left;
                else
                    return null;
            }
        }

        /// <summary>
        /// Returns a float representing the current value of the right trigger.
        /// The value has the range of 0 - 1.0, depending on the depth of application.
        /// </summary>
        public float? RightTriggerDepth {
            get {
                if (m_padConnected)
                    return m_curPad.Triggers.Right;
                else
                    return null;
            }
        }

        /// <summary>
        /// Returns a vector representing the current position of the left thumbstick.
        /// Ranges from -1.0 - 1.0, with zero at the centre. Up and right are positive.
        /// </summary>
        public Vector2? LeftThumbstickPosition {
            get {
                if (m_padConnected)
                    return m_curPad.ThumbSticks.Left;
                else
                    return null;
            }
        }

        /// <summary>
        /// Returns a vector representing the current position of the right thumbstick.
        /// Ranges from -1.0 - 1.0, with zero at the centre. Up and right are positive.
        /// </summary>
        public Vector2? RightThumbstickPosition {
            get {
                if (m_padConnected)
                    return m_curPad.ThumbSticks.Right;
                else
                    return null;
            }
        }

        /***** DEBUG *****/

        /// <summary>
        /// Returns a double that represents the division of the scroll-
        /// wheel delta value by the current scroll-wheel value.
        /// </summary>
        public double DeltaDiv {
            get { return m_deltaDivisor; }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Updates the logic of all the input devices within the game.
        /// </summary>
        /// <param name="currentPlayer">
        /// Represents the index of the current player using an input device.
        /// </param>
        public void updateInputDevices(PlayerIndex currentPlayer) {
            // Save Previous input device states and values
            m_prevKB = m_curKB;
            m_prevMouse = m_curMouse;
            m_prevPad = m_curPad;
            m_prevScrVal = m_curScrVal;

            // Get the current input device states and values
            m_curKB = Keyboard.GetState();
            m_curMouse = Mouse.GetState();
            m_curPad = GamePad.GetState(currentPlayer);
            m_curScrVal = m_curMouse.ScrollWheelValue;

            // Set the current player's game pad connection flag
            m_padConnected = m_curPad.IsConnected;

            // Calculate mouse co-ordinate and scroll value deltas
            m_mouseDeltaX = (m_prevMouse.X - m_curMouse.X);
            m_mouseDeltaY = (m_prevMouse.Y - m_curMouse.Y);
            m_scrDelta = (m_prevScrVal - m_curScrVal);
            /***** DEBUG *****/
            //m_deltaDivisor = (m_scrDelta / m_curScrVal);
        }

        /***** Mouse Buttons *****/

        /// <summary>
        /// Detects whether the Left Mouse Button is pressed down.
        /// </summary>
        /// <returns>
        /// True, if the mouse button is currently down.
        /// False otherwise.
        /// </returns>
        public bool isLeftMouseButtonPressed() {
            return m_curMouse.LeftButton.HasFlag(ButtonState.Pressed);
        }

        /// <summary>
        /// Detects whether the Left Mouse Button has been tapped.
        /// </summary>
        /// <returns>
        /// True, if the mouse button has just been tapped (pressed, then released).
        /// False otherwise.
        /// </returns>
        public bool isLeftMouseButtonTapped() {
            return (m_curMouse.LeftButton.HasFlag(ButtonState.Pressed) &&
                m_prevMouse.LeftButton.HasFlag(ButtonState.Released));
        }

        /// <summary>
        /// Detects whether the Middle Mouse Button is pressed down.
        /// </summary>
        /// <returns>
        /// True, if the mouse button is currently down.
        /// False otherwise.
        /// </returns>
        public bool isMiddleMouseButtonPressed() {
            return m_curMouse.MiddleButton.HasFlag(ButtonState.Pressed);           
        }

        /// <summary>
        /// Detects whether the Middle Mouse Button has been tapped.
        /// </summary>
        /// <returns>
        /// True, if the mouse button has just been tapped (pressed, then released).
        /// False otherwise.
        /// </returns>
        public bool isMiddleMouseButtonTapped() { 
            return (m_curMouse.MiddleButton.HasFlag(ButtonState.Pressed) &&
                m_prevMouse.MiddleButton.HasFlag(ButtonState.Released));
        }

        /// <summary>
        /// Detects whether the Right Mouse Button is pressed down.
        /// </summary>
        /// <returns>
        /// True, if the mouse button is currently down.
        /// False otherwise.
        /// </returns>
        public bool isRightMouseButtonPressed() {
            return m_curMouse.RightButton.HasFlag(ButtonState.Pressed);
        }

        /// <summary>
        /// Detects whether the Right Mouse Button has been tapped.
        /// </summary>
        /// <returns>
        /// True, if the mouse button has just been tapped (pressed, then released).
        /// False otherwise.
        /// </returns>
        public bool isRightMouseButtonTapped() {
            return (m_curMouse.RightButton.HasFlag(ButtonState.Pressed) &&
                m_prevMouse.RightButton.HasFlag(ButtonState.Released)); 
        }

        /***** Xbox 360 Controller *****/

        /// <summary>
        /// Detects whether the Left D-Pad is being pressed down. 
        /// </summary>
        /// <returns>
        /// True, if the d-pad is currently down.
        /// False otherwise.
        /// </returns>
        public bool isLeftDPadPressed() {
            if (m_padConnected) {
                return m_curPad.DPad.Left.HasFlag(ButtonState.Pressed);
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Left D-Pad has been tapped.
        /// </summary>
        /// <returns>
        /// True, if the pad has just been tapped (pressed, then released).
        /// False otherwise.
        /// </returns>
        public bool isLeftDPadTapped() {
            if (m_padConnected) {
                return (m_curPad.DPad.Left.HasFlag(ButtonState.Pressed) &&
                    m_prevPad.DPad.Left.HasFlag(ButtonState.Released));
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Up D-Pad is being pressed down.
        /// </summary>
        /// <returns>
        /// True, if the d-pad is currently down.
        /// False otherwise.
        /// </returns>
        public bool isUpDPadPressed() {
            if (m_padConnected) {
                return m_curPad.DPad.Up.HasFlag(ButtonState.Pressed);
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Up D-Pad has been tapped. 
        /// </summary>
        /// <returns>
        /// True, if the pad has just been tapped (pressed, then released).
        /// False otherwise.
        /// </returns>
        public bool isUpDPadTapped() {
            if (m_padConnected) {
                return (m_curPad.DPad.Up.HasFlag(ButtonState.Pressed) &&
                    m_prevPad.DPad.Up.HasFlag(ButtonState.Released));
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Right D-Pad is being pressed.
        /// </summary>
        /// <returns>
        /// True, if the d-pad is currently down.
        /// False otherwise.
        /// </returns>
        public bool isRightDPadPressed() {
            if (m_padConnected) {
                return m_curPad.DPad.Right.HasFlag(ButtonState.Pressed);
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Right D-Pad has been tapped.
        /// </summary>
        /// <returns>
        /// True, if the pad has just been tapped (pressed, then released).
        /// False otherwise.
        /// </returns>
        public bool isRightDPadTapped() {
            if (m_padConnected) {
                return (m_curPad.DPad.Right.HasFlag(ButtonState.Pressed) &&
                    m_prevPad.DPad.Right.HasFlag(ButtonState.Released));
            }
            return false;
        }

        /// <summary>
        /// Detects whther the Down D-Pad is being pressed.
        /// </summary>
        /// <returns>
        /// True, if the d-pad is currently down.
        /// False otherwise.
        /// </returns>
        public bool isDownDPadPressed() {
            if (m_padConnected) {
                return m_curPad.DPad.Down.HasFlag(ButtonState.Pressed);
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Down D-Pad has been tapped.
        /// </summary>
        /// <returns>
        /// True, if the d-pad has just been tapped (pressed, then released).
        /// False otherwise.
        /// </returns>
        public bool isDownDPadTapped() {
            if (m_padConnected) {
                return (m_curPad.DPad.Down.HasFlag(ButtonState.Pressed) &&
                    m_prevPad.DPad.Down.HasFlag(ButtonState.Released));
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Left Trigger is down.
        /// </summary>
        /// <returns>
        /// True on the value of the trigger being greater than zero.
        /// False otherwise.
        /// </returns>
        public bool isLeftTriggerPressed() {
            if (m_padConnected) {
                return m_curPad.Triggers.Left > 0;
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Right Trigger is down.
        /// </summary>
        /// <returns>
        /// True on the value of the trigger being greater than zero.
        /// False otherwise.
        /// </returns>
        public bool isRightTriggerPressed() {
            if (m_padConnected) {
                return m_curPad.Triggers.Right > 0;
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Left Bumper is being pressed.
        /// </summary>
        /// <returns>
        /// True, if the bumper is currently being pressed.
        /// False otherwise.
        /// </returns>
        public bool isLeftBumperPressed() {
            if (m_padConnected) {
                return m_curPad.Buttons.LeftShoulder.HasFlag(ButtonState.Pressed);
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Left Bumper has been tapped.
        /// </summary>
        /// <returns>
        /// True, if the bumper has just been tapped (pressed, then released).
        /// False otherwise.
        /// </returns>
        public bool isLeftBumperTapped() {
            if (m_padConnected) {
                return (m_curPad.Buttons.LeftShoulder.HasFlag(ButtonState.Pressed) &&
                    m_prevPad.Buttons.LeftShoulder.HasFlag(ButtonState.Released));
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Right Bumper is being pressed.
        /// </summary>
        /// <returns>
        /// True, if the bumper is currently being pressed.
        /// False otherwise.
        /// </returns>
        public bool isRightBumperPressed() {
            if (m_padConnected) {
                return m_curPad.Buttons.RightShoulder.HasFlag(ButtonState.Pressed);
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Right Bumper has been tapped.
        /// </summary>
        /// <returns>
        /// True, if the bumper has just been tapped (pressed, then released).
        /// False otherwise.
        /// </returns>
        public bool isRightBumperTapped() {
            if (m_padConnected) {
                return (m_curPad.Buttons.RightShoulder.HasFlag(ButtonState.Pressed) &&
                    m_prevPad.Buttons.RightShoulder.HasFlag(ButtonState.Released));
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Back Button is being pressed.
        /// </summary>
        /// <returns>
        /// True, if the button is currently being pressed.
        /// False otherwise.
        /// </returns>
        public bool isBackPressed() {
            if (m_padConnected) {
                return m_curPad.Buttons.Back.HasFlag(ButtonState.Pressed);
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Back Button has been tapped.
        /// </summary>
        /// <returns>
        /// True, if the button has just been tapped (pressed, the released).
        /// False otherwise.
        /// </returns>
        public bool isBackTapped() {
            if (m_padConnected) {
                return (m_curPad.Buttons.Back.HasFlag(ButtonState.Pressed) &&
                    m_prevPad.Buttons.Back.HasFlag(ButtonState.Released));
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Start Button is being pressed.
        /// </summary>
        /// <returns>
        /// True, if the button is currently being pressed.
        /// False otherwise.
        /// </returns>
        public bool isStartPressed() {
            if (m_padConnected) {
                return m_curPad.Buttons.Start.HasFlag(ButtonState.Pressed);
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Start Button has been tapped.
        /// </summary>
        /// <returns>
        /// True, if the button has just been tapped (pressed, the released).
        /// False otherwise.
        /// </returns>
        public bool isStartTapped() {
            if (m_padConnected) {
                return (m_curPad.Buttons.Start.HasFlag(ButtonState.Pressed) &&
                    m_prevPad.Buttons.Start.HasFlag(ButtonState.Released));
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Xbox Guide Button is being pressed.
        /// </summary>
        /// <returns>
        /// True, if the button is currently being pressed.
        /// False otherwise.
        /// </returns>
        public bool isGuidePressed() {
            if (m_padConnected) {
                return m_curPad.Buttons.BigButton.HasFlag(ButtonState.Pressed);
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Xbox Guide Button has been tapped.
        /// </summary>
        /// <returns>
        /// True, if the button has just been tapped (pressed, the released).
        /// False otherwise.
        /// </returns>
        public bool isGuideTapped() {
            if (m_padConnected) {
                return (m_curPad.Buttons.BigButton.HasFlag(ButtonState.Pressed) &&
                    m_prevPad.Buttons.BigButton.HasFlag(ButtonState.Released));
            }
            return false;
        }

        /// <summary>
        /// Detects whether the A Button is being pressed.
        /// </summary>
        /// <returns>
        /// True, if the button is currently being pressed.
        /// False otherwise.
        /// </returns>
        public bool isAPressed() {
            if (m_padConnected) {
                return m_curPad.Buttons.A.HasFlag(ButtonState.Pressed);
            }
            return false;
        }

        /// <summary>
        /// Detects whether the A Button has been tapped.
        /// </summary>
        /// <returns>
        /// True, if the button has just been tapped (pressed, the released).
        /// False otherwise.
        /// </returns>
        public bool isATapped() {
            if (m_padConnected) {
                return (m_curPad.Buttons.A.HasFlag(ButtonState.Pressed) &&
                    m_prevPad.Buttons.A.HasFlag(ButtonState.Released));
            }
            return false;
        }

        /// <summary>
        /// Detects whether the X Button is being pressed.
        /// </summary>
        /// <returns>
        /// True, if the button is currently being pressed.
        /// False otherwise.
        /// </returns>
        public bool isXPressed() {
            if (m_padConnected) {
                return m_curPad.Buttons.X.HasFlag(ButtonState.Pressed);
            }
            return false;
        }

        /// <summary>
        /// Detects whether the X Button has been tapped.
        /// </summary>
        /// <returns>
        /// True, if the button has just been tapped (pressed, the released).
        /// False otherwise.
        /// </returns>
        public bool isXTapped() {
            if (m_padConnected) {
                return (m_curPad.Buttons.X.HasFlag(ButtonState.Pressed) &&
                    m_prevPad.Buttons.X.HasFlag(ButtonState.Released));
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Y Button is being pressed.
        /// </summary>
        /// <returns>
        /// True, if the button is currently being pressed.
        /// False otherwise.
        /// </returns>
        public bool isYPressed() {
            if (m_padConnected) {
                return m_curPad.Buttons.Y.HasFlag(ButtonState.Pressed);
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Y Button has been tapped.
        /// </summary>
        /// <returns>
        /// True, if the button has just been tapped (pressed, the released).
        /// False otherwise.
        /// </returns>
        public bool isYTapped() {
            if (m_padConnected) {
                return (m_curPad.Buttons.Y.HasFlag(ButtonState.Pressed) &&
                    m_prevPad.Buttons.Y.HasFlag(ButtonState.Released));
            }
            return false;
        }

        /// <summary>
        /// Detects whether the B Button is being pressed.
        /// </summary>
        /// <returns>
        /// True, if the button is currently being pressed.
        /// False otherwise.
        /// </returns>
        public bool isBPressed() {
            if (m_padConnected) {
                return m_curPad.Buttons.B.HasFlag(ButtonState.Pressed);
            }
            return false;
        }

        /// <summary>
        /// Detects whether the B Button has been tapped.
        /// </summary>
        /// <returns>
        /// True, if the button has just been tapped (pressed, the released).
        /// False otherwise.
        /// </returns>
        public bool isBTapped() {
            if (m_padConnected) {
                return (m_curPad.Buttons.B.HasFlag(ButtonState.Pressed) &&
                    m_prevPad.Buttons.B.HasFlag(ButtonState.Released));
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Left Thumbstick is being pressed.
        /// </summary>
        /// <returns>
        /// True, if the stick is currently being pressed.
        /// False otherwise.
        /// </returns>
        public bool isLeftThumbstickPressed() {
            if (m_padConnected) {
                return m_curPad.Buttons.LeftStick.HasFlag(ButtonState.Pressed);
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Left Thumbstick has been tapped.
        /// </summary>
        /// <returns>
        /// True, if the stick has just been tapped (pressed, the released).
        /// False otherwise.
        /// </returns>
        public bool isLeftThumbstickTapped() {
            if (m_padConnected) {
                return (m_curPad.Buttons.LeftStick.HasFlag(ButtonState.Pressed) &&
                    m_prevPad.Buttons.LeftStick.HasFlag(ButtonState.Released));
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Right Thumbstick is being pressed.
        /// </summary>
        /// <returns>
        /// True, if the stick is currently being pressed.
        /// False otherwise.
        /// </returns>
        public bool isRightThumbstickPressed() {
            if (m_padConnected) {
                return m_curPad.Buttons.RightStick.HasFlag(ButtonState.Pressed);
            }
            return false;
        }

        /// <summary>
        /// Detects whether the Right Thumbstick has been tapped.
        /// </summary>
        /// <returns>
        /// True, if the stick has just been tapped (pressed, the released).
        /// False otherwise.
        /// </returns>
        public bool isRightThumbstickTapped() {
            return
                m_padConnected ?
                (m_curPad.Buttons.RightStick.HasFlag(ButtonState.Pressed) &&
                m_prevPad.Buttons.RightStick.HasFlag(ButtonState.Released)) :
                false;
        }

        /// <summary>
        /// Detects whether the left thumbstick is pushed to the left.
        /// </summary>
        /// <returns>
        /// True, if the value of the thumbstick's X coordinate is less than zero.
        /// False otherwise.
        /// </returns>
        public bool isLeftThumbstickFacingLeft() {
            return m_padConnected ? (LeftThumbstickPosition.Value.X < 0) : false;
        }

        /// <summary>
        /// Detects whether the left thumbstick is pushed to the right.
        /// </summary>
        /// <returns>
        /// True, if the value of the thumbstick's X coordinate is more than zero.
        /// False otherwise.
        /// </returns>
        public bool isLeftThumbstickFacingRight() {
            return m_padConnected ? (LeftThumbstickPosition.Value.X > 0) : false;
        }

        /// <summary>
        /// Detects whether the left thumbstick is pushed up.
        /// </summary>
        /// <returns>
        /// True, if the value of the thumbstick's Y coordinate is more than zero.
        /// False otherwise.
        /// </returns>
        public bool isLeftThumbstickFacingUp() {
            return m_padConnected ? (LeftThumbstickPosition.Value.Y > 0) : false;
        }

        /// <summary>
        /// Detects whether the left thumbstick is pushed down.
        /// </summary>
        /// <returns>
        /// True, if the value of the thumbstick's Y coordinate is less than zero.
        /// False otherwise.
        /// </returns>
        public bool isLeftThumbstickFacingDown() {
            return m_padConnected ? (LeftThumbstickPosition.Value.Y < 0) : false;
        }

        /// <summary>
        /// Detects whether the left thumbstick is in the dead zone (central).
        /// </summary>
        /// <returns>
        /// True, if the left thumbstick vector is equal to a zero vector.
        /// False otherwise.
        /// </returns>
        public bool isLeftThumbstickDeadZone() {
            return m_padConnected ? LeftThumbstickPosition.Value.Equals(Vector2.Zero) : false;
        }

        /// <summary>
        /// Detects whether the right thumbstick is pushed to the left.
        /// </summary>
        /// <returns>
        /// True, if the value of the thumbstick's X coordinate is less than zero.
        /// False otherwise.
        /// </returns>
        public bool isRightThumbstickFacingLeft() { 
            return m_padConnected ? (RightThumbstickPosition.Value.X < 0) : false;
        }

        /// <summary>
        /// Detects whether the right thumbstick is pushed to the right.
        /// </summary>
        /// <returns>
        /// True, if the value of the thumbstick's X coordinate is more than zero.
        /// False otherwise.
        /// </returns>
        public bool isRightThumbstickFacingRight() {
            return m_padConnected ? (RightThumbstickPosition.Value.X > 0) : false;
        }

        /// <summary>
        /// Detects whether the right thumbstick is pushed up.
        /// </summary>
        /// <returns>
        /// True, if the value of the thumbstick's Y coordinate is more than zero.
        /// False otherwise.
        /// </returns>
        public bool isRightThumbstickFacingUp() {
            return m_padConnected ? (RightThumbstickPosition.Value.Y > 0) : false;
        }

        /// <summary>
        /// Detects whether the right thumbstick is pushed down.
        /// </summary>
        /// <returns>
        /// True, if the value of the thumbstick's Y coordinate is less than zero.
        /// False otherwise.
        /// </returns>
        public bool isRightThumbstickFacingDown() {
            return m_padConnected ? (RightThumbstickPosition.Value.Y < 0) : false;
        }

        /// <summary>
        /// Detects whether the right thumbstick is in the dead zone (central).
        /// </summary>
        /// <returns>
        /// True, if the right thumbstick vector is equal to a zero vector.
        /// False otherwise.
        /// </returns>
        public bool isRightThumbstickDeadZone() {
            return m_padConnected ? RightThumbstickPosition.Value.Equals(Vector2.Zero) : false;
        }

        /***** Keys *****/

        /// <summary>
        /// Detects if a key is being pressed.
        /// </summary>
        /// <param name="key">
        /// Represents one of the many keys on the keyboard.
        /// </param>
        /// <returns>
        /// True, if the key is currently down.
        /// False otherwise.
        /// </returns>
        public bool isKeyPressed(Keys key) {
            return m_curKB.IsKeyDown(key);
        }

        /// <summary>
        /// Detects if the key was recently tapped (pressed, then released).
        /// </summary>
        /// <param name="key">
        /// Represents one of the many keys on the keyboard.
        /// </param>
        /// <returns>
        /// True, if the key was tapped.
        /// False otherwise.
        /// </returns>
        public bool isKeyTapped(Keys key) {
            return (m_curKB.IsKeyDown(key) && m_prevKB.IsKeyUp(key));
        }

        #endregion
    }

    #endregion
}
