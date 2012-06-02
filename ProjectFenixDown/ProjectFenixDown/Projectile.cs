using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectFenixDown
{
    public class Projectile : TheShredder
    {
        const int MAX_DISTANCE = 400;

        public bool Visible = false;

        Vector2 _startPosition;

        public void LoadContent(ContentManager contentManager, String textureName, int damage)
        {
            base.LoadContent(contentManager, textureName);
            _scale = 0.7f;
            _damage = damage;

        }

        public void Update(GameTime gameTime, Player player)
        {
            if (Vector2.Distance(_startPosition, _position) > MAX_DISTANCE)
                Visible = false;
            if (Visible == true)
                base.UpdateProjectile(gameTime, _speed, _direction);
            
            CheckHit(player);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Visible == true)
                base.Draw(spriteBatch);
        }

        public void Fire(Vector2 startPosition, Vector2 speed, Vector2 direction)
        {
            _position = startPosition;
            _startPosition = startPosition;
            _speed = speed;
            _direction = direction;
            Visible = true;
        }

        public void CheckHit(Player player)
        {

            if (Source.Intersects(player.Source))
            {
                Visible = false;
                player._health -= _damage;
            }

        }
    }
}
