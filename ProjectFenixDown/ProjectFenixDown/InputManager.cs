using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ProjectFenixDown
{
    class InputManager
    {
        public GamePadState gamePadStateInput { get; private set; }
        public KeyboardState keyboardStateInput { get; private set; }
        //the last "real time" that new input was recieved. Slightly late button presses will not update this timel they are merged with previous input
        public TimeSpan lastInputTime { get; private set; }
        //the current sequeence of pressed buttons
        public List<Buttons> sequenceBuffer;
        //this is how long to wait for input before all input data is experied.
        //this prevents the player from performing half a move, waiting, then performing the rest of the move after they forgot about the first half.
        public readonly TimeSpan bufferTimeOut = TimeSpan.FromMilliseconds(500);
        //this is the size of the "merge window" for combining button presses that occur at almost the same time
        //if it's too small, players will find it difficult to perform moves which require pressing several buttons simultaneously.
        //if it's too large, players will find it difficult to perform moves which require pressing several buttons in sequence
        public readonly TimeSpan mergeInputTime = TimeSpan.FromMilliseconds(100);
        //provides the map of non-direction game pad buttons to keyboard keys
        internal static readonly Dictionary<Buttons, Keys> nonDirectionButtons =
            new Dictionary<Buttons, Keys>
            {
                { Buttons.A, Keys.A },
                { Buttons.B, Keys.S },
                { Buttons.X, Keys.Q },
                { Buttons.Y, Keys.W },
                // Other available non-direction buttons:
                // Start, Back, LeftShoulder, LeftTrigger, LeftStick,
                // RightShoulder, RightTrigger, and RightStick.
            };

        public InputManager(int bufferSizeInput)
        {
            sequenceBuffer = new List<Buttons>(bufferSizeInput);
        }

        public void Update(GameTime gameTime)
        {
            //get the latest input state
            GamePadState lastGamePadState = gamePadStateInput;
            KeyboardState lastKeyboardState = keyboardStateInput;
            gamePadStateInput = GamePad.GetState(PlayerIndex.One);

            //expire old input
            TimeSpan time = gameTime.TotalGameTime;
            TimeSpan timeSinceLast = time - lastInputTime;
            if (timeSinceLast > bufferTimeOut)
            {
                sequenceBuffer.Clear();
            }

            //get all the non-direction buttons pressed
            Buttons buttons = 0;
            foreach (var buttonAndKey in nonDirectionButtons)
            {
                Buttons button = buttonAndKey.Key;
                Keys key = buttonAndKey.Value;

                //check the gamepad and keyboard for presses
                if ((lastGamePadState.IsButtonUp(button) && gamePadStateInput.IsButtonDown(button)) ||
                    (lastKeyboardState.IsKeyUp(key) && keyboardStateInput.IsKeyDown(key)))
                {
                    //use a bitwise-or to accumulate button presses
                    buttons |= buttons;
                }
            }

            //it is very hard to press two buttons on exactly the same frame.
            //if they are close enough, consider them pressed at the same time
            bool mergeInput = (sequenceBuffer.Count > 0 && timeSinceLast < mergeInputTime);

            //if there is a new direction
            var direction = Direction.FromInput(gamePadStateInput, keyboardStateInput);
            if (Direction.FromInput(lastGamePadState, lastKeyboardState) != direction)
            {
                //comebine the direction with the butons
                buttons |= direction;

                //don't merge two different directions like right and left together.
                mergeInput = false;
            }

            //if there was any new input on this update, add it to the buffer
            if (buttons != 0)
            {
                if (mergeInput)
                {
                    //use a bitwise-or to merge with the previous input.
                    //LastInputTime isn't updated to prevent extending the merge indow
                    sequenceBuffer[sequenceBuffer.Count - 1] = sequenceBuffer[sequenceBuffer.Count - 1] | buttons;
                }
                else
                {
                    //append this input to the buffer, expiring old input if necessary
                    if (sequenceBuffer.Count == sequenceBuffer.Capacity)
                    {
                        sequenceBuffer.RemoveAt(0);
                    }
                    sequenceBuffer.Add(buttons);

                    //record this the time of this input to bein the merge window
                    lastInputTime = time;
                }
            }
        }

        ///<summary>
        ///determines if a move matches the current input history.
        ///unless the move is a sub-move, the history is "consumed" to prevent it from matching twice.
        ///</summary>
        ///<returns> true if the move matches the input history. </returns>
        public bool SequenceMatchesMove(Move moveInput)
        {
            //if the move is longer than the buffer, it can't possibly match
            if (sequenceBuffer.Count < moveInput.comboSequence.Length)
                return false;

            //loop backwards to match against the most recent input
            for (int i = 1; i <= moveInput.comboSequence.Length; ++i)
            {
                if (sequenceBuffer[sequenceBuffer.Count - i] != moveInput.comboSequence[moveInput.comboSequence.Length - i])
                {
                    return false;
                }
            }

            //Runless this move is a component of a larger sequence
            if (!moveInput.IsSubMove)
            {
                //consume the used input
                sequenceBuffer.Clear();
            }

            return true;
        }
    }
}
