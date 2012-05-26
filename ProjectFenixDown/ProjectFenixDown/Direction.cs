using System;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace ProjectFenixDown
{
    /// <summary>
    /// Helper class for working with the 8-way directions stored in a buttons enum.
    /// </summary>
    class Direction
    {
        //helper bit masks for directios defined with the buttons flags enum
        public const Buttons None = 0;
        public const Buttons Up = Buttons.DPadUp | Buttons.LeftThumbstickUp;
        public const Buttons Down = Buttons.DPadDown | Buttons.LeftThumbstickDown;
        public const Buttons Left = Buttons.DPadLeft | Buttons.LeftThumbstickLeft;
        public const Buttons Right = Buttons.DPadRight | Buttons.LeftThumbstickRight;
        //probably unncessary
        /*
        public const Buttons UpLeft = Up | Left;
        public const Buttons UpRight = Up | Right;
        public const Buttons DownLeft = Down | Left;
        public const Buttons DownRight = Down | Right;
        */
        public const Buttons Any = Up | Down | Left | Right;

        /// <summary>
        /// gets the current direction from a gamepad and keyboard;
        /// </summary>
        public static Buttons FromInput(GamePadState gamePadInput, KeyboardState keyboardInput)
        {
            Buttons direction = None;

            //get vertical direction
            if (gamePadInput.IsButtonDown(Buttons.DPadUp) || gamePadInput.IsButtonDown(Buttons.LeftThumbstickUp) || keyboardInput.IsKeyDown(Keys.Up))
            {
                direction |= Up;
            }
            else if (gamePadInput.IsButtonDown(Buttons.DPadDown) || gamePadInput.IsButtonDown(Buttons.LeftThumbstickDown) || keyboardInput.IsKeyDown(Keys.Down))
            {
                direction |= Down;
            }

            //combine with horizontal direction
            if (gamePadInput.IsButtonDown(Buttons.DPadLeft) || gamePadInput.IsButtonDown(Buttons.LeftThumbstickLeft) || keyboardInput.IsKeyDown(Keys.Left))
            {
                direction |= Left;
            }
            else if (gamePadInput.IsButtonDown(Buttons.DPadRight) || gamePadInput.IsButtonDown(Buttons.LeftThumbstickRight) || keyboardInput.IsKeyDown(Keys.Right))
            {
                direction |= Right;
            }

            return direction;
        }

        ///<summary>
        ///Gets the direciton without non-direction buttons from a set of buttons flags
        ///</summary>
        public static Buttons FromButtons(Buttons buttons)
        {
            //extract the direction from a full set of buttons using a bit mask
            return buttons & Any;
        }
    }
}
