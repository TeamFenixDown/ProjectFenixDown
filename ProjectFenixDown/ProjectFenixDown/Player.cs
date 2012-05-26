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

        //current user movement input
        private float playerMovementInput;

        private Rectangle playerLocalBounds;
        //get a rectangle which bounds this player in world space
        public Rectangle playerBoundingBox
        {
            get
            {
                int left = (int)Math.Round(playerPosition.X ) + playerLocalBounds.X;
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

        // Input configuration
        private const float MoveStickScale = 1.5f;

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

            loadContent();

            //sets the player movement speed
            velocity = Vector2.Zero;
            
        }

        public void loadContent()
        {

            //calculates the player's local bounds
            int width = (int)(tempPlayerTexture.Width * 0.7);
            int left = (int)(tempPlayerTexture.Width - width) / 2;
            int height = (int)(tempPlayerTexture.Height * 0.9);
            int top = (int)(tempPlayerTexture.Height - height);
            playerLocalBounds = new Rectangle(left, top, width, height);
        }

        public void Update(GameTime gameTimeInput, KeyboardState keyboardStateInput, GamePadState gamepadStateInput)
        {
            //update the input
            InputUpdate(gameTimeInput, keyboardStateInput, gamepadStateInput);
            ApplyPhysics(gameTimeInput);

            // Clear input.
            playerMovementInput = 0.0f;
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
            //get analog horizontal movement
            playerMovementInput = gamepadStateInput.ThumbSticks.Left.X * MoveStickScale;

            //ignore small movements to prevent running in place.
            if (Math.Abs(playerMovementInput) < 0.5f)
                playerMovementInput = 0.0f;

            //if any digital movement input is found, override the analog movment
            if (gamepadStateInput.IsButtonDown(Buttons.DPadLeft) || keyboardStateInput.IsKeyDown(Keys.Left))
            {
                playerMovementInput = -1.5f;
            }
            else if (gamepadStateInput.IsButtonDown(Buttons.DPadRight) || keyboardStateInput.IsKeyDown(Keys.Right))
            {
                playerMovementInput = 1.5f;

            }
        }

        public void ApplyPhysics(GameTime gameTimeInput)
        {
            float elapsed = (float)gameTimeInput.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = playerPosition;

            //base velocity is a combination of horizontal movement control and acceleration downward due to gravity
            velocity.X += playerMovementInput * moveAcceleration * elapsed;
            velocity.Y = MathHelper.Clamp(velocity.Y + gravityAcceleration * elapsed, -maxFallSpeed, maxFallSpeed);

            //apply pseudo-drag horizontally
            if (isOnGround)
                velocity.X *= groundDragFactor;
            else
                velocity.X *= airDragFactor;

            //prevent the player from running faster than her top speed
            velocity.X = MathHelper.Clamp(velocity.X, -maxMoveSpeed, maxMoveSpeed);

            //apply velocity
            playerPosition += velocity * elapsed;
            playerPosition = new Vector2((float)Math.Round(playerPosition.X), (float)Math.Round(playerPosition.Y));

            //if the player is now colliding with the level, separate them
            HandleCollisions();

            //if the collision stopped us from moving, reset the velocity to zero
            if (playerPosition.X == previousPosition.X)
                velocity.X = 0;
            if (playerPosition.Y == previousPosition.Y)
                velocity.Y = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tempPlayerTexture, playerPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
