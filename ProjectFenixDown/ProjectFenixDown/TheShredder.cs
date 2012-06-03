using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectFenixDown
{
    public class TheShredder : Enemy
    {

        const String TEXTURENAME = "shredder";
        protected List<Projectile> _projectiles = new List<Projectile>();
        ContentManager _contentManager;
        double shootingDelay;

        public override void Initialize(Player player, Level level, Vector2 position, int health, int movementSpeed, int damage, int exp)
        {
            _player = player;

            base.Initialize(level, position, health, damage, movementSpeed, exp);
        }
        public override void LoadContent(ContentManager contentManager)
        {
            _contentManager = contentManager;

            foreach (Projectile projectile in _projectiles)
            {
                projectile.LoadContent(contentManager, "Hadouken");
            }
            base.LoadContent(contentManager, TEXTURENAME);
        }

        public override void Update(GameTime gameTime)
        {
            UpdateMovement(gameTime);
            Updateprojectile(gameTime);

            base.Update(gameTime);
        }

        public override void UpdateMovement(GameTime gameTime)
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

        public void UpdateProjectile(GameTime gameTime, Vector2 speed, Vector2 direction)
        {
            _position += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Updateprojectile(GameTime gameTime)
        {
            foreach (Projectile projectile in _projectiles)
            {
                projectile.Update(gameTime, _player);
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

            Vector2 playerCenter = new Vector2(_player._position.X - _player.Source.Width / 2, _player._position.Y + (_player.Source.Height / 2));
            Vector2 projectileCenter = new Vector2(_position.X + Source.Width / 2, _position.Y + Source.Height / 2);
            Vector2 attackVector = playerCenter - projectileCenter;
            attackVector.Normalize();

            if (_projectiles.Count <= 20)
            {
                if (_currentState != State.Stunned)
                {
                    Projectile projectile = new Projectile();
                    _projectiles.Add(projectile);
                    projectile.LoadContent(_contentManager, "hadouken", 10);
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
