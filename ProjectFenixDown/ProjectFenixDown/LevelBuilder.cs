using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace ProjectFenixDown
{
    class LevelBuilder
    {
        private Random random = new Random(354668);
        // Physical structure of the level.
        public Tile[,] tilesGrid;

        // Level content.        
        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        //width of the level measured in tiles
        public int gridWidth
        {
            get { return tilesGrid.GetLength(0); }
        }
        //height of the level measured in tiles
        public int gridHeight
        {
            get { return tilesGrid.GetLength(1); }
        }

        public void Initialize(IServiceProvider serviceProvider, Stream fileStream, String levelName)
        {
            //create a new content manager to load content used just by this level
            content = new ContentManager(serviceProvider, "Content");
            LoadTiles(fileStream);
        }

        //iterates over every tile in the strutre file and loads its appearance and behavior.
        private void LoadTiles(Stream fileStream)
        {
            //load the level and ensure all the lines are the same length
            int width;
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    line = reader.ReadLine();
                }
            }

            //allocate the tile grid
            tilesGrid = new Tile[width, lines.Count];

            //loop over every tile position
            for (int y = 0; y < gridHeight; ++y)
            {
                for (int x = 0; x < gridWidth; ++x)
                {
                    //to load each tile
                    char tileType = lines[y][x];
                    tilesGrid[x, y] = LoadTile(tileType, x, y);
                }
            }
        }

        private Tile LoadTile(char tileTypeInput, int xInput, int yInput)
        {
            switch (tileTypeInput)
            {
                //blank space
                case '.':
                    return new Tile(null, TileCollision.passable);

                case '#':
                    return LoadEnviornmentTile("Ground", 4, TileCollision.impassable);

                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileTypeInput, xInput, yInput));
            }
        }

        private Tile LoadEnviornmentTile(string tileNameInput, int variationCount, TileCollision collisionInput)
        {
            int index = random.Next(variationCount);
            return LoadTile(tileNameInput + index, collisionInput);
        }

        private Tile LoadTile(string name, TileCollision collisionInput)
        {
            return new Tile(content.Load<Texture2D>("Tiles/" + name), collisionInput);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawEnviornmentTiles(spriteBatch);
        }

        //draws all the Enviornment tiles in the level
        private void DrawEnviornmentTiles(SpriteBatch spriteBatch)
        {
            //for each tile positon
            for (int y = 0; y < gridHeight; ++y)
            {
                for (int x = 0; x < gridWidth; ++x)
                {
                    //if there is a visible tile in that position
                    Texture2D texture = tilesGrid[x, y].texture;
                    if (texture != null)
                    {
                        //draw it in screen space
                        Vector2 position = new Vector2(x, y) * Tile.size;
                        spriteBatch.Draw(texture, position, Color.White);
                    }
                }
            }
        }
    }
}
