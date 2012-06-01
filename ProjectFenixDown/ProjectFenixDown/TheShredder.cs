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

        protected List<Projectile> _projectiles = new List<Projectile>();
        ContentManager _contentManager;
        double shootingDelay;

        State _currentState = State.Walking;
        State _playerState;

        public void LoadContent(ContentManager contentManager, String textureName)
        {
            _contentManager = contentManager;

            foreach (Projectile projectile in _projectiles)
            {
                projectile.LoadContent(contentManager, "Hadouken");
            }
            base.LoadContent(contentManager, textureName);
        }

        public void Update(GameTime gameTime)
        {
            UpdateMovement(gameTime);
            Updateprojectile(gameTime);

            ApplyPhysics(gameTime);
            _speed = Vector2.Zero;
        }

        public void UpdateMovement(GameTime gameTime)
        {
            for (int y = _topTile; y <= _bottomTile; ++y)
            {
                for (int x = _leftTile; x <= _rightTile; ++x)
                {
                    TileCollision collision = _level.GetCollision((int)(x + _speed.X), y);
                    if (collision != TileCollision.passable)
                    {
                        _speed.X = _movementSpeed;
                    }
                    else { _movementSpeed *= -1; }
                }
            }
                
        }

        public void Updateprojectile(GameTime gameTime)
        {
            foreach (Projectile projectile in _projectiles)
            {
                projectile.Update(gameTime);
            }
            for (int i = 0; i < _projectiles.Count; i++)
            {
                if (_projectiles[i].Visible == false)
                {
                    _projectiles.RemoveAt(i);
                    i--;
                }
            }


            if (shootingDelay > 0)
                shootingDelay -= gameTime.ElapsedGameTime.TotalSeconds;
            else
            {
                Shootprojectile(gameTime);
            }

        }

        private void Shootprojectile(GameTime gameTime)
        {
            shootingDelay = .5;
            float projectileSpeed = 400;

            Vector2 attackVector = _playerPosition - _position;
            attackVector.Normalize();

            if (_projectiles.Count <= 20)
            {
                if (_currentState == State.Walking)
                {
                    Projectile projectile = new Projectile();
                    _projectiles.Add(projectile);
                    projectile.LoadContent(_contentManager, "hadouken");
                    projectile.Fire(_position + new Vector2(_size.Width / 2, _size.Height / 2),
                                new Vector2(projectileSpeed, projectileSpeed), attackVector);
                }
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
