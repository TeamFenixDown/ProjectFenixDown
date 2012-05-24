using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectFenixDown
{
    class Player
    {
        //variables

        //animations
        public Texture2D tempPlayerTexture;

        //position of the player relative to the upper left side of the screen
        public Vector2 position;

        //state of the player, is she alive?
        public bool isAlive;

        //amount of hitpoints the player has
        public int health;

        //Get the Width of the player ship
        public int Width
        {
            get { return tempPlayerTexture.Width; }
        }
        //get the height of the player ship
        public int Height
        {
            get { return tempPlayerTexture.Height; }
        }

        public void Initialize(Texture2D textureInput, Vector2 positionInput)
        {
            tempPlayerTexture = textureInput;

            //set the starting position of the player around the middle of the scrren adn to the back
            position = positionInput;

            //set the player to be alive
            isAlive = true;

            //set the player health
            health = 100;
        }

        public void Update()
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tempPlayerTexture, position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
