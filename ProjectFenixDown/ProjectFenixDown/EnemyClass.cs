using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectFenixDown
{
    public class Enemy : Character
    {


        protected Player _player;

        ContentManager _contentManager;

        public virtual void Initialize(Player player, Level level, Vector2 position, int health, int movementSpeed, int damage, int exp)
        {
            _player = player;

            base.Initialize(level, position, health, damage, movementSpeed, exp);
        }
        public virtual void LoadContent(ContentManager contentManager)
        {
            _contentManager = contentManager;

            base.LoadContent(contentManager, "tempPlayer");
        }

        public override void Update(GameTime gameTime)
        {
            UpdateMovement(gameTime);

            base.Update(gameTime);
        }

        public virtual void UpdateMovement(GameTime gameTime)
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

        public override void Draw(SpriteBatch spriteBatch)
        {

            base.Draw(spriteBatch);
        }

    }
}
