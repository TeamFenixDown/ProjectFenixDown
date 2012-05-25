using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectFenixDown
{
    class Player
    {
        //variables
        //animations
        public Texture2D tempPlayerTexture;

        //position of the player relative to the upper left side of the screen
        public Vector2 position;
        //state of the player, is she alive?
        public bool isAlive;
        //amount of hitpoints the player has
        public int health;
        //Get the Width of the player
        public int width
        {
            get { return tempPlayerTexture.Width; }
        }
        //get the height of the player
        public int height
        {
            get { return tempPlayerTexture.Height; }
        }

        //Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        //gamepad states used to determine button presses
        GamePadState currentGamepadState;
        GamePadState previousGamepadState;

        //a movement speed for the player
        float playerMoveSpeed;

        public void Initialize(Texture2D textureInput, Vector2 positionInput)
        {
            tempPlayerTexture = textureInput;

            //set the starting position of the player around the middle of the scrren adn to the back
            position = positionInput;

            //set the player to be alive
            isAlive = true;

            //set the player health
            health = 100;

            //sets the player movement speed
            playerMoveSpeed = 8.0f;
        }

        public void Update(GameTime gameTimeInput, KeyboardState keyboardStateInput, GamePadState gamepadStateInput)
        {
            //update the input
            InputUpdate(gameTimeInput, keyboardStateInput, gamepadStateInput);
        }

        public void InputUpdate(GameTime gameTimeInput, KeyboardState keyboardStateInput, GamePadState gamepadStateInput)
        {
            // Save the previous state of the keyboard and game pad so we can determine single key/button presses
            previousGamepadState = gamepadStateInput;
            previousKeyboardState = keyboardStateInput;

            //get thumbstick controls
            position.X += gamepadStateInput.ThumbSticks.Left.X * playerMoveSpeed;
            position.Y -= gamepadStateInput.ThumbSticks.Left.Y * playerMoveSpeed;

            //use the keyboard/Dpad
            if (keyboardStateInput.IsKeyDown(Keys.Left) || gamepadStateInput.DPad.Left == ButtonState.Pressed)
            {
                position.X -= playerMoveSpeed;
            }
            if (keyboardStateInput.IsKeyDown(Keys.Right) || gamepadStateInput.DPad.Right == ButtonState.Pressed)
            {
                position.X += playerMoveSpeed;
            }
            if (keyboardStateInput.IsKeyDown(Keys.Up) || gamepadStateInput.DPad.Up == ButtonState.Pressed)
            {
                position.Y -= playerMoveSpeed;
            }
            if (keyboardStateInput.IsKeyDown(Keys.Down) || gamepadStateInput.DPad.Down == ButtonState.Pressed)
            {
                position.Y += playerMoveSpeed;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tempPlayerTexture, position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
