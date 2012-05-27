using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectFenixDown
{
    class TheShredder : Character
    {

        List<Projectile> _projectiles = new List<Projectile>();
        ContentManager _contentManager;

        State _currentState = State.Walking;
        State _playerState;

        const string TEXTURENAME = "shredder";
        const int START_POSITION_X = 125;
        const int START_POSITION_Y = 245;
        const int SPEED = 20;

        Vector2 _direction = Vector2.Zero;
        Vector2 _speed = Vector2.Zero;

        public void LoadContent(ContentManager contentManager, String textureName)
        {
            _contentManager = contentManager;

            foreach (Projectile projectile in _projectiles)
            {
                projectile.LoadContent(contentManager, "Hadouken");
            }
            _position = new Vector2(START_POSITION_X, START_POSITION_Y);
            base.LoadContent(contentManager, textureName);
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            UpdateMovement(currentKeyboardState);
            Updateprojectile(gameTime, currentKeyboardState);
            _previousKeyboardState = currentKeyboardState;
            base.Update(gameTime, new Vector2(_movementSpeed, _movementSpeed), new Vector2(1, 0));
        }

        public void UpdateMovement(KeyboardState keyboardState)
        {
            
        }

        public void Updateprojectile(GameTime gameTime, KeyboardState currentKeyboardState)
        {
            foreach (Projectile projectile in _projectiles)
            {
                projectile.Update(gameTime);
            }

            if (currentKeyboardState.IsKeyDown(Keys.Space) == true && _previousKeyboardState.IsKeyDown(Keys.Space) == false)
            {
                Shootprojectile(gameTime);
            }
        }

        private void Shootprojectile(GameTime gameTime)
        {

            float projectileSpeed = 200;

            Vector2 attackVector = _playerPosition - _position;
            attackVector.Normalize();

            if (_currentState == State.Walking)
            {
                Projectile projectile = new Projectile();
                projectile.LoadContent(_contentManager, "hadouken");
                projectile.Fire(_position + new Vector2(_size.Width / 2, _size.Height / 2),
                            new Vector2(projectileSpeed, projectileSpeed), attackVector);
                _projectiles.Add(projectile);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            foreach (Projectile projectile in _projectiles)
            {
                projectile.Draw(spriteBatch);
            }

            base.Draw(spriteBatch);
        }

    }
}
