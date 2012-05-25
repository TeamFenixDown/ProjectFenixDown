using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace ProjectFenixDown
{
    class SampleLevel
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
