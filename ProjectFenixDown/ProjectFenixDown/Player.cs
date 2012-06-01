using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectFenixDown
{
    class Player : Character
    {
        // Input configuration
        private const float MoveStickScale = 1.5f;

        //Variables for input handling
        //The master list of moves for the player
        Move[] _moves;
        //the move list used for move detection at runtime
        MovesList _currentMoveList;

        //the move list is used to match against an input manager for the player
        InputManager _inputManager;
        //stores the player's most recent move and when they pressed it.
        Move _playerMostRecentMove;
        TimeSpan _playerMostRecentMoveTime;

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

            loadContent();

            //sets the player movement speed
            _velocity = Vector2.Zero;

            //constructs the list of moves
            _moves = new Move[]
                {
                    new Move("RunLeft",     Direction.Left) { IsSubMove = true },
                    new Move("RunRight",    Direction.Right) { IsSubMove = true },
                    new Move("SprintLeft",  Direction.Left, Direction.Left),
                    new Move("SprintRight", Direction.Right, Direction.Right),
                    //new Move("Jump",        Buttons.A) { IsSubMove = true },
                };
            //constructs a move list which will store its own copy of the moves array.
            _currentMoveList = new MovesList(_moves);

            //create an InputManager for the player with a sufficiently large buffer
            _inputManager = new InputManager(_currentMoveList.LongestMoveLength);
            
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
            _inputManager.Update(gameTimeInput);
            

            // Dection and record the currents player's move recent move
            Move newMove = _currentMoveList.DetectMove(_inputManager);
            if (newMove != null)
            {
                _playerMostRecentMove = newMove;
                _playerMostRecentMoveTime = gameTimeInput.TotalGameTime;
            }


            if (_playerMostRecentMove != null)
            {
                PerformMove(_playerMostRecentMove, keyboardStateInput, gamepadStateInput);
            }

            //temp stuff to test jumping
            _isJumping = gamepadStateInput.IsButtonDown(Buttons.A);

            base.Update(gameTimeInput);
        }

        public void PerformMove(Move moveToPerform, KeyboardState keyboardStateInput, GamePadState gamepadStateInput)
        {
            if (moveToPerform.name == "RunLeft")
            {
                _speed.X = -1.25f;
                if (gamepadStateInput.IsButtonUp(Buttons.DPadLeft) && gamepadStateInput.IsButtonUp(Buttons.LeftThumbstickLeft) && keyboardStateInput.IsKeyUp(Keys.Left))
                {
                    _speed.X = 0;
                    _playerMostRecentMove = null;
                }
            }
            if (moveToPerform.name == "RunRight")
            {
                _speed.X = 1.25f;
                if (gamepadStateInput.IsButtonUp(Buttons.DPadRight) && gamepadStateInput.IsButtonUp(Buttons.LeftThumbstickRight) && keyboardStateInput.IsKeyUp(Keys.Right))
                {
                    _speed.X = 0;
                    _playerMostRecentMove = null;
                }
            }
            if (moveToPerform.name == "SprintLeft")
            {
                _speed.X = -1.75f;
                if (gamepadStateInput.IsButtonUp(Buttons.DPadLeft) && gamepadStateInput.IsButtonUp(Buttons.LeftThumbstickLeft) && keyboardStateInput.IsKeyUp(Keys.Left))
                {
                    _speed.X = 0;
                    _playerMostRecentMove = null;
                }
            }
            if (moveToPerform.name == "SprintRight")
            {
                _speed.X = 1.75f;
                if (gamepadStateInput.IsButtonUp(Buttons.DPadRight) && gamepadStateInput.IsButtonUp(Buttons.LeftThumbstickRight) && keyboardStateInput.IsKeyUp(Keys.Right))
                {
                    _speed.X = 0;
                    _playerMostRecentMove = null;
                }
            }
            if (moveToPerform.name == "Jump")
            {
                _isJumping = true;
                /*if (gamepadStateInput.IsButtonUp(Buttons.A) && keyboardStateInput.IsKeyUp(Keys.A))
                {
                    isJumping = false;
                    playerMostRecentMove = null;
                }*/

            }
        }


    }
}
