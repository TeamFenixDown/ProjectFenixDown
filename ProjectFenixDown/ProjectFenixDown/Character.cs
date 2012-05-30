using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectFenixDown
{
    //this will become the superclass for all character types
    //each subclass will be an enemy subtype (or the player)
    //however, bosses will likely require their own unique class
    public class Character : Game1
    {
        protected enum State
        {
            Idle,
            Walking,
            WalkingLeft,
            WalkingRight,
            Jumping,
            Stunned,
            Poisoned,
            Possessed,
            Blind
        }

        //variables
        //animations
        protected Texture2D _texture;
        public String _textureName;
        protected State _currentState;
        protected KeyboardState _previousKeyboardState;
        protected GamePadState _previousGamePadState;

        protected Rectangle _source;
        public Rectangle Source
        {
            get
            {
                int left = (int)Math.Round(_position.X) + _source.X;
                int top = (int)Math.Round(_position.Y) + _source.Y;

                return new Rectangle(left, top, _source.Width, _source.Height);
            }
        }

        public Vector2 _position;
        public bool _isAlive;
        public int _health;
        public int _exp;
        public int _damage;
        //Speed is along the X,Y axis respectively. Useful for fixed axis or arc motion
        public Vector2 _speed;
        public Vector2 _direction;
        public Rectangle _size;
        protected float _scale = 1.0f;

        public bool _isOnGround;
        protected float _previousBottom;
        //velocity of the player
        public Vector2 _velocity;
        protected float _moveAcceleration = 13000.0f;
        protected float _maxMoveSpeed = 1750.0f;
        private const float _gravityAcceleration = 3400.0f;
        private const float _maxFallSpeed = 700.0f;
        private const float _jumpControlPower = 0.17f;
        private const float _groundDragFactor = 0.48f;
        private const float _airDragFactor = 0.48f;
        private float _maxJumpTime = 0.35f;
        private float _jumpLaunchVelocity = -3500.0f; 

        //jumping state
        protected bool _isJumping;
        private bool _wasJumping;
        private float _jumpTime;

        //Check Surrounding tiles
        protected int _topTile, _bottomTile, _leftTile, _rightTile;


        //get the level we're working on
        Level Level
        {
            get { return _level; }
        }
        public Level _level;

        //Get the Width of the player
        public int _width
        {
            get { return _texture.Width; }
        }
        //get the height of the player
        public int _height
        {
            get { return _texture.Height; }
        }

        //a movement speed for the player
        protected float _movementSpeed;

        public void Initialize(Level levelInput, Texture2D texture, Vector2 position, int health, int damage, int movementSpeed, int exp)
        {
            this._level = levelInput;
            _texture = texture;
            _position = position;
            _isAlive = true;
            _health = 100;
            _damage = 10;
            _movementSpeed = 3.0f;
            _exp = 100;

        }

        public void Update(GameTime gameTime, Vector2 speed, Vector2 direction)
        {
            //_position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            ApplyPhysics(gameTime);
            
        }

        public void UpdateProjectile(GameTime gameTime, Vector2 speed, Vector2 direction)
        {
            _position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public void LoadContent(ContentManager contentManager, string textureName)
        {
            _texture = contentManager.Load<Texture2D>(textureName);
            _textureName = textureName;
            _source = new Rectangle(0, 0, _texture.Width, _texture.Height);

            _size = new Rectangle(0, 0, (int)(_texture.Width * _scale), (int)(_texture.Height * _scale));
        }

        public float Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                //Recalculate size of sprite with new value
                _size = new Rectangle(0, 0, (int)(_texture.Width * _scale), (int)(_texture.Height * _scale));
            }
        }

        public void setPlayerInformation(Vector2 playerPosition, Vector2 playerSpeed, Vector2 playerDirection)
        {
            _playerPosition = playerPosition;
            _playerSpeed = playerSpeed;
            _playerDirection = playerDirection;
        }
        public void ApplyPhysics(GameTime gameTimeInput)
        {
            float elapsed = (float)gameTimeInput.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = _position;

            //base velocity is a combination of horizontal movement control and acceleration downward due to gravity NEEDS UPWARDS
            _velocity.X += _speed.X * _moveAcceleration * elapsed;
            _velocity.Y = MathHelper.Clamp(_velocity.Y + _gravityAcceleration * elapsed, -_maxFallSpeed, _maxFallSpeed);

            _velocity.Y = DoJump(_velocity.Y, gameTimeInput);

            //apply pseudo-drag horizontally
            if (_isOnGround)
                _velocity.X *= _groundDragFactor;
            else
                _velocity.X *= _airDragFactor;

            //prevent the player from running faster than her top speed
            _velocity.X = MathHelper.Clamp(_velocity.X, -_maxMoveSpeed, _maxMoveSpeed);

            //apply velocity
            _position += _velocity * elapsed;
            _position = new Vector2((float)Math.Round(_position.X), (float)Math.Round(_position.Y));

            //if the player is now colliding with the level, separate them
            HandleCollisions();

            //if the collision stopped us from moving, reset the velocity to zero
            if (_position.X == previousPosition.X)
                _velocity.X = 0;
            if (_position.Y == previousPosition.Y)
                _velocity.Y = 0;
        }

        /// <summary>
        /// Calculates the Y velocity for jumping and animates accordingly
        /// </summary>
        /// <remarks>
        /// During the ascent of a jump, the y velocity is comepltely overridden by a powercurve.
        /// During the Descent, gravity takes over.
        /// The jump velocity is controlled by the jumpTime field which measures time into the ascent of the current jump
        /// </remarks>
        /// <param name="velocityY">
        /// The player's current velocity along the Y axis.
        /// </param>
        /// <returns>
        /// A new Y velocity if beginning or continuing a jump.
        /// Otherwise, the existing Y velocity.
        /// </returns>
        protected float DoJump(float velocityY, GameTime gameTimeInput)
        {
            //if the player wants to do a jump
            if (_isJumping)
            {
                //begin or continue a jump
                if ((!_wasJumping && _isOnGround) || _jumpTime > 0.0f)
                {
                    if (_jumpTime == 0.0f)
                    {
                        //play jump sound here
                    }

                    _jumpTime += (float)gameTimeInput.ElapsedGameTime.TotalSeconds;
                    //play the animation for jumping here

                }

                //if we are in the ascent of the jump
                if (0.0f < _jumpTime && _jumpTime <= _maxJumpTime)
                {
                    //fully override the vertical velocity with a powercurve that gives players move control over the top of the jump
                    velocityY = _jumpLaunchVelocity * (1.0f - (float)Math.Pow(_jumpTime / _maxJumpTime, _jumpControlPower));
                }
                else
                {
                    //reached the apex of the jump
                    _jumpTime = 0.0f;
                }
            }
            else
            {
                //continues not jumping or cancels a jump in progress
                _jumpTime = 0.0f;
            }
            _wasJumping = _isJumping;

            return velocityY;
        }

        public void HandleCollisions()
        {
            //get the player's bounding rectangle and find neighboring tiles
            Rectangle playerBounds = Source;
            _leftTile = (int)Math.Floor((float)playerBounds.Left / Tile.width);
            _rightTile = (int)Math.Ceiling((float)playerBounds.Right / Tile.width);
            _topTile = (int)Math.Floor((float)playerBounds.Top / Tile.height);
            _bottomTile = (int)Math.Ceiling((float)playerBounds.Bottom / Tile.height);

            //reset flag to search for ground collision
            _isOnGround = false;

            //for each potentially colliding tile,
            for (int y = _topTile; y <= _bottomTile; ++y)
            {
                for (int x = _leftTile; x <= _rightTile; ++x)
                {
                    //if this tile is collidable
                    TileCollision collision = _level.GetCollision(x, y);
                    if (collision != TileCollision.passable)
                    {
                        //determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = _level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(playerBounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            //resolve the collision along the shallow axis
                            if (absDepthY < absDepthX || collision == TileCollision.platform)
                            {
                                //if we crossed the top of a tile, we are on the ground
                                if (_previousBottom <= tileBounds.Top)
                                    _isOnGround = true;

                                //ignore platforms, unless we are on the ground
                                if (collision == TileCollision.impassable || _isOnGround)
                                {
                                    //resolve the collision along the y axis
                                    _position = new Vector2(_position.X, _position.Y + depth.Y);

                                    playerBounds = Source;
                                }
                            }
                            //ignore platforms
                            else if (collision == TileCollision.impassable)
                            {
                                //resolve the collision along the X axis
                                _position = new Vector2(_position.X + depth.X, _position.Y);

                                //perform further collisions with the new bounds
                                playerBounds = Source;
                            }
                        }
                    }
                }
            }
            //save the new bounds bottom
            _previousBottom = playerBounds.Bottom;
        }
    }
}
