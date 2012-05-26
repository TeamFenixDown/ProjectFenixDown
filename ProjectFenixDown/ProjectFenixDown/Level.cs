using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace ProjectFenixDown
{
    class Level
    {
        //the level builder
        LevelBuilder levelBuilder;

        public void Initialize(IServiceProvider serviceProvider, Stream fileStream, String levelName)
        {
            //initialize the level builder
            levelBuilder = new LevelBuilder();
            levelBuilder.Initialize(serviceProvider, fileStream, levelName);
        }

        public void LoadContent()
        {
        }

        //gets the collision mode of the tile at a particular locations
        //This method handles tiles outside of the levels boundaries by making it impossible to escape past the left or right edges,
        //but allowing things to jump beyond the top of the level, and fall off the bottom
        public TileCollision GetCollision(int x, int y)
        {
            //prevents the escape past the level ends
            if (x < 0 || x >= levelBuilder.gridWidth)
                return TileCollision.impassable;
            if (y < 0 || y >= levelBuilder.gridHeight)
                return TileCollision.passable;

            return levelBuilder.tilesGrid[x, y].collision;
        }

        //get the counding rectangle of a tile in world space
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.width, y * Tile.height, Tile.width, Tile.height);
        }
        
        public void Update()
        {
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //draw the level
            levelBuilder.Draw(gameTime, spriteBatch);
        }
    }
}
