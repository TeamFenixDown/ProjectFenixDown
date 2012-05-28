using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectFenixDown
{
    class Projectile : TheShredder
    {
        const int MAX_DISTANCE = 400;

        public bool Visible = false;

        Vector2 _startPosition;
        Vector2 _speed;
        Vector2 _direction;

        public void LoadContent(ContentManager contentManager, String textureName)
        {
            base.LoadContent(contentManager, textureName);
            _scale = 0.7f;
        }

        public void Update(GameTime gameTime)
        {
            if (Vector2.Distance(_startPosition, _position) > MAX_DISTANCE)
                Visible = false;
            if (Visible == true)
                base.UpdateProjectile(gameTime, _speed, _direction);
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
    }
}
