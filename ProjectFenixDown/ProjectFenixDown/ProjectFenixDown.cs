using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ProjectFenixDown
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // We store our input states so that we only poll once per frame, 
        // then we use the same input state wherever needed
        private GamePadState gamePadState;
        private KeyboardState keyboardState;

        //represents the player
        Player playerCharacter;
        protected Vector2 _playerPosition;
        protected Vector2 _playerDirection;
        protected Vector2 _playerSpeed;

        //represents the sample level
        Level sampleLevel;

        //First Enemy
        TheShredder enemyCharacter;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Initialize the player class
            playerCharacter = new Player();
            sampleLevel = new Level();
            enemyCharacter = new TheShredder();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //////////////////////////test load sample level test////////////////////////////////////
            string levelName = "SampleLevel";
            string levelPath = string.Format("Content/Levels/{0}.txt", levelName);
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
                sampleLevel.Initialize(Services, fileStream, levelName);

            // Load the player resources 
            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            playerCharacter.Initialize(sampleLevel, Content.Load<Texture2D>("tempPlayer"), playerPosition);
            enemyCharacter.LoadContent(this.Content, "tempPlayer");
            enemyCharacter.Initialize(sampleLevel, Content.Load<Texture2D>("tempPlayer"), new Vector2(250, 100), 100, 1, 20, 100);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            // Handle polling for our input
            HandleInput();
            playerCharacter.Update(gameTime, keyboardState, gamePadState);
            enemyCharacter.Update(gameTime);
            
            //Player information
            _playerPosition = playerCharacter._position;
            _playerSpeed = playerCharacter._speed;
            _playerDirection = playerCharacter._direction;
            enemyCharacter.setPlayerInformation(_playerPosition, _playerSpeed, _playerDirection);
            //playerClamp();
            

            base.Update(gameTime);
        }

        private void playerClamp()
        {
            // make sure that the player does not go out of bounds
            playerCharacter._position.X = MathHelper.Clamp(playerCharacter._position.X, 0, GraphicsDevice.Viewport.Width - playerCharacter._width);
            playerCharacter._position.Y = MathHelper.Clamp(playerCharacter._position.Y, 0, GraphicsDevice.Viewport.Height - playerCharacter._height);
        }

        private void HandleInput()
        {
            //get all of our input states
            keyboardState = Keyboard.GetState();
            gamePadState = GamePad.GetState(PlayerIndex.One);

            // Exit the game when back is pressed.
            if (gamePadState.Buttons.Back == ButtonState.Pressed)
                Exit();

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Start drawing
            spriteBatch.Begin();

            //draw the level
            sampleLevel.Draw(gameTime, spriteBatch);

            // Draw the Player
            enemyCharacter.Draw(this.spriteBatch);
            playerCharacter.Draw(spriteBatch);
            

            // Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
