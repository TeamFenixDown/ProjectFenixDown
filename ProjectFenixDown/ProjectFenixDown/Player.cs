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

        //state of the player, is she alive?
        public bool isAlive;
        //amount of hitpoints the player has
        public int health;

        // Get the width of the player ship
        public int width
        {
            get { return tempPlayerTexture.Width; }
        }

        // Get the height of the player ship
        public int height
        {
            get { return tempPlayerTexture.Height; }
        }
        
        //position of the player relative to the upper left side of the screen
        public Vector2 position;
        //velocity of the player
        public Vector2 velocity;

        //constants for controlling horizontal movement
        private float moveAcceleration = 13000.0f;
        private float maxMoveSpeed = 1750.0f;
        private const float groundDragFactor = 0.48f;
        private const float airDragFactor = 0.58f;

        //constants for controlling vertical movement
        private float maxJumpTime = 0.35f;
        private float jumpLaunchVelocity = -3500.0f;
        private const float gravityAcceleration = 3400.0f;
        private const float maxFallSpeed = 550.0f;
        private const float jumpControlPower = 0.14f;

        //is the player on the gruond?
        public bool isOnGround;

        //jumping state
        private bool isJumping;
        private bool wasJumping;
        private float jumpTime;

        //Keyboard states used to determine key presses
        KeyboardState previousKeyboardState;
        //gamepad states used to determine button presses
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

        public void loadContent()
        {
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
