using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectFenixDown
{
    class Player : Character
    {

        //constants for controlling vertical movement
        private float maxJumpTime = 0.35f;
        private float jumpLaunchVelocity = -3500.0f;        

        private float _previousBottom;

        //jumping state
        private bool isJumping;
        private bool wasJumping;
        private float jumpTime;

        // Input configuration
        private const float MoveStickScale = 1.5f;

        public void Initialize(Level levelInput ,Texture2D textureInput, Vector2 positionInput)
        {
            this._level = levelInput;

            _texture = textureInput;

            //set the starting position of the player around the middle of the scrren and to the back
            _position = positionInput;

            //set the player to be alive
            _isAlive = true;

            //set the player health
            _health = 100;

            _moveAcceleration = 13000.0f;
            _maxMoveSpeed = 1750.0f;

            loadContent();

            //sets the player movement speed
            _velocity = Vector2.Zero;
            
        }

        public void loadContent()
        {

            //calculates the player's local bounds
            int width = (int)(_texture.Width * 0.7);
            int left = (int)(_texture.Width - width) / 2;
            int height = (int)(_texture.Height * 0.9);
            int top = (int)(_texture.Height - height);
            _source = new Rectangle(left, top, width, height);
        }

        public void Update(GameTime gameTimeInput, KeyboardState keyboardStateInput, GamePadState gamepadStateInput)
        {
            //update the input
            InputUpdate(gameTimeInput, keyboardStateInput, gamepadStateInput);
            ApplyPhysics(gameTimeInput);

            // Clear input.
            _speed = Vector2.Zero;
        }

        public void InputUpdate(GameTime gameTimeInput, KeyboardState keyboardStateInput, GamePadState gamepadStateInput)
        {
            //get analog horizontal movement
            _speed.X = gamepadStateInput.ThumbSticks.Left.X * MoveStickScale;

            //ignore small movements to prevent running in place.
            if (Math.Abs(_speed.X) < 0.5f)
                _speed.X = 0.0f;

            //if any digital movement input is found, override the analog movment
            if (gamepadStateInput.IsButtonDown(Buttons.DPadLeft) || keyboardStateInput.IsKeyDown(Keys.Left))
            {
                _speed.X = -1.5f;
            }
            else if (gamepadStateInput.IsButtonDown(Buttons.DPadRight) || keyboardStateInput.IsKeyDown(Keys.Right))
            {
                _speed.X = 1.5f;
            }
            if(keyboardStateInput.IsKeyDown(Keys.Up) == true)
                _speed.Y = 1.5f;
        }
    }
}
