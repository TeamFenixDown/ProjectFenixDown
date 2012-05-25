using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
namespace ProjectFenixDown
{
    class Animation
    {
        ///varriables
        //the image representing the collection of images used for animation
        Texture2D spriteStrip;
        //the scale used to display the spritestrip
        float scale;
        //the time since we last updated the frame
        int elapsedTime;
        //the time we display a frame until the next one
        int frameTime;
        //the number of frames that the animation contains
        int frameCount;
        //the index of the current frame we are displaying
        int currentFrame;
        //the color of the frame we will be displaying
        Color color;

        //the area of the image strip we want to display
        Rectangle sourceRect = new Rectangle();
        //the area where we want to display the image strip in the game
        Rectangle destinationRect = new Rectangle();

        //width of a given frame
        public int frameWidth;
        //height of a given frame
        public int frameHeight;

        //the state of the animation
        public bool active;
        //determines if the animation will keep playing or deactive after one run
        public bool looping;

        //width of a given frame
        public Vector2 position;

        //gets the texture origin at the bottom center of each frame
        public Vector2 origin
        {
            get { return new Vector2(frameWidth / 2.0f, frameHeight); }
        }

        public void Initialize(Texture2D textureInput, Vector2 positionInput, int frameWidthInput, int frameHeightInput, int frameCountInput, int frameTimeInput, Color colorInput, float scaleInput, bool loopingInput)
        {
            //keep a local copy of the values passed on
            this.color = colorInput;
            this.frameHeight = frameHeightInput;
            this.frameWidth = frameWidthInput;
            this.frameCount = frameCountInput;

            looping = loopingInput;
            position = positionInput;
            spriteStrip = textureInput;

            //set the time to zero
            elapsedTime = 0;
            currentFrame = 0;

            //set the animation to active by default
            active = true;
        }

        public void Update(GameTime gameTime)
        {
            //do not update the game if we are not active
            if (active == false)
                return;

            //update the elapsed time
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            //if the slapsed time is larger than the frame time, we need to switch frames
            if (elapsedTime > frameTime)
            {
                //move to the next frame
                currentFrame++;

                //if the currentFrame is equal to the frameCount, reset the currentFrame to zero
                if (currentFrame == frameCount)
                {
                    currentFrame = 0;
                    //if we are not looping deactivate the animation
                    if (looping == false)
                        active = false;
                }

                //reset the elapsed time to zero
                elapsedTime = 0;
            }

            //grab the correct frame in the image strip by multiplying the currentframe index by the frame width
            sourceRect = new Rectangle((int)currentFrame * frameWidth, 0, frameWidth, frameHeight);

            //grab the correct frame in the image strip by multiplying the current frame index by the frame width
            destinationRect = new Rectangle((int)position.X - (int)(frameWidth * scale) / 2, (int)position.Y - (int)(frameHeight * scale) / 2, (int)(frameWidth * scale), (int)(frameHeight * scale));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //only draw the animation when we are active
            if (active)
            {
                spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);
            }
        }
    }
}
