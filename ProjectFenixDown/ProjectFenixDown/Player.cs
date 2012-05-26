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

        // Get the width of the player
        public int width
        {
            get { return tempPlayerTexture.Width; }
        }

        // Get the height of the player
        public int height
        {
            get { return tempPlayerTexture.Height; }
        }
        
        //position of the player relative to the upper left side of the screen
        public Vector2 playerPosition;

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

        private float previousBottom;

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

        private Rectangle playerLocalBounds;
        //get a rectangle which bounds this player in world space
        public Rectangle playerBoundingBox
        {
            get
            {
                int left = (int)Math.Round(playerPosition.X) + playerLocalBounds.X;
                int top = (int)Math.Round(playerPosition.Y) + playerLocalBounds.Y;

                return new Rectangle(left, top, playerLocalBounds.Width, playerLocalBounds.Height);
            }
        }

        //get the level we're working on
        public Level Level
        {
            get { return level; }
        }
        Level level;

        public void Initialize(Level levelInput ,Texture2D textureInput, Vector2 positionInput)
        {
            this.level = levelInput;

            tempPlayerTexture = textureInput;

            //set the starting position of the player around the middle of the scrren and to the back
            playerPosition = positionInput;

            //set the player to be alive
            isAlive = true;

            //set the player health
            health = 100;

            //sets the player movement speed
            playerMoveSpeed = 7.0f;

            loadContent();
        }

        public void loadContent()
        {

            //calculates the player's local bounds
            int width = (int)(tempPlayerTexture.Width);
            int left = (int)(tempPlayerTexture.Width - width);
            int height = (int)(tempPlayerTexture.Height * 0.9);
            int top = (int)(tempPlayerTexture.Height - height);
            playerLocalBounds = new Rectangle(left, top, width, height);
        }

        public void Update(GameTime gameTimeInput, KeyboardState keyboardStateInput, GamePadState gamepadStateInput)
        {
            //update the input
            InputUpdate(gameTimeInput, keyboardStateInput, gamepadStateInput);
            HandleCollisions();
        }

        public void HandleCollisions()
        {
            //get the player's bounding rectangle and find neighboring tiles
            Rectangle playerBounds = playerBoundingBox;
            int leftTile = (int)Math.Floor((float)playerBounds.Left / Tile.width);
            int rightTile = (int)Math.Ceiling((float)playerBounds.Right / Tile.width);
            int topTile = (int)Math.Floor((float)playerBounds.Top / Tile.height);
            int bottomTile = (int)Math.Ceiling((float)playerBounds.Bottom / Tile.height);

            //reset flag to search for ground collision
            isOnGround = false;

            //for each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    //if this tile is collidable
                    TileCollision collision = level.GetCollision(x, y);
                    if (collision != TileCollision.passable)
                    {
                        //determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(playerBounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            //resolve the collision along the shallow axis
                            if (absDepthY < absDepthX || collision == TileCollision.platform)
                            {
                                //if we crossed the top of a tile, we are on the ground
                                if (previousBottom <= tileBounds.Top)
                                    isOnGround = true;

                                //ignore platforms, unless we are on the ground
                                if (collision == TileCollision.impassable || isOnGround)
                                {
                                    //resolve the collision along the y axis
                                    playerPosition = new Vector2(playerPosition.X, playerPosition.Y + depth.Y);

                                    playerBounds = playerBoundingBox;
                                }
                            }
                            //ignore platforms
                            else if (collision == TileCollision.impassable)
                            {
                                //resolve the collision along the X axis
                                playerPosition = new Vector2(playerPosition.X + depth.X, playerPosition.Y);

                                //perform further collisions with the new bounds
                                playerBounds = playerBoundingBox;
                            }
                        }
                    }
                }
            }
            //save the new bounds bottom
            previousBottom = playerBounds.Bottom;
        }

        public void InputUpdate(GameTime gameTimeInput, KeyboardState keyboardStateInput, GamePadState gamepadStateInput)
        {
            // Save the previous state of the keyboard and game pad so we can determine single key/button presses
            previousGamepadState = gamepadStateInput;
            previousKeyboardState = keyboardStateInput;

            //get thumbstick controls
            playerPosition.X += gamepadStateInput.ThumbSticks.Left.X * playerMoveSpeed;
            playerPosition.Y -= gamepadStateInput.ThumbSticks.Left.Y * playerMoveSpeed;

            //use the keyboard/Dpad
            if (keyboardStateInput.IsKeyDown(Keys.Left) || gamepadStateInput.DPad.Left == ButtonState.Pressed)
            {
                playerPosition.X -= playerMoveSpeed;
            }
            if (keyboardStateInput.IsKeyDown(Keys.Right) || gamepadStateInput.DPad.Right == ButtonState.Pressed)
            {
                playerPosition.X += playerMoveSpeed;
            }
            if (keyboardStateInput.IsKeyDown(Keys.Up) || gamepadStateInput.DPad.Up == ButtonState.Pressed)
            {
                playerPosition.Y -= playerMoveSpeed;
            }
            if (keyboardStateInput.IsKeyDown(Keys.Down) || gamepadStateInput.DPad.Down == ButtonState.Pressed)
            {
                playerPosition.Y += playerMoveSpeed;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tempPlayerTexture, playerPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
