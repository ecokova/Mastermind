#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace Thing3
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
    enum Role
    {
        Encoder,
        Decoder
    }
    public class Game1 : Game
    {

        const int WINDOW_WIDTH = 600;
        const int WINDOW_HEIGHT = 500;
        const int NUM_DIFFICULTIES = 3;
        const int NUM_ROLES = 2;

        enum GameState
        {
            Intro,            // "Splash page"
            Config,           // Selecting difficulty, role, etc
            Instructions,   
            Playing,
            GameOver
        }
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState keyboard;
        KeyboardState oldKeyboard;
        GameState gameState;

        Texture2D imgLogo;
        Texture2D imgDifficulties;
        Texture2D imgRoles;
        Texture2D imgSelector;

        MastermindGame game;
        Difficulty difficulty;
        Role role;

        long animationTimer;
        int menuItemCounter;

        public Game1()
            : base()
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
            gameState = GameState.Intro;
            animationTimer = 0;
            menuItemCounter = 0;
            role = Role.Encoder;
            difficulty = Difficulty.Easy;

            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            graphics.ApplyChanges();

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

            imgLogo = this.Content.Load<Texture2D>("tmpLogo");
            imgDifficulties = this.Content.Load<Texture2D>("tmpDifficulties");
            imgRoles = this.Content.Load<Texture2D>("tmpRoles");

            //NEEDSWORK: This will eventually be a triangle or some other "indicator"
            imgSelector = this.Content.Load<Texture2D>("SoftOrb");
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
            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();

            oldKeyboard = keyboard;
            keyboard = Keyboard.GetState();

            switch (gameState)
            {
                  
                case GameState.Intro:
                    animationTimer++;
                    if (animationTimer == 60 * 2)
                    {
                        animationTimer = 0;
                        gameState = GameState.Config;
                    }
                    break;
                case GameState.Config:

                    #region Unsuccessful code change notes
                    /*-------------------------------------------------------------------
                     * Works, but flips 60 times a second. Would need to check that those
                     * conditions AREN'T true for oldKeyboard, but that would make the 
                     * if statement almost impossible to read. Scrapping this idea and
                     * just moving the if statements to a function.
                     *-------------------------------------------------------------------
                    // NEEDSWORK: CLEAN THIS UP
                    // NOTE: LeftShift seems to be in every list of pressed keys if a
                    // key was held rather than pressed. It comes second in the list, 
                    // so if a key was pressed, it will be the first one.
                    Keys[] pressed = keyboard.GetPressedKeys();
                    if (pressed.Length > 0 && pressed[0] != Keys.LeftShift)
                    {
                        // NOTE: Relative key values:
                        //  Up:    1
                        //  Right: 0
                        //  Down: -1
                        //  Left: -2
                        
                        int upDownOffset = (int)(Keys.Right);
                        menuItemCounter = (menuItemCounter + ((int)pressed[0] - upDownOffset)) % 2;
                        
                    }
                    */
                    #endregion
                    if (wasKeyPressed(Keys.Up) || wasKeyPressed(Keys.Down))
                    {
                        switchMenuItem(keyboard.GetPressedKeys()[0], 2);
                    }
                    if (wasKeyPressed(Keys.Right) || wasKeyPressed(Keys.Left))
                    {
                        if (menuItemCounter == 0)   
                            switchChosenRole(keyboard.GetPressedKeys()[0]);
                        else
                            switchChosenDifficulty(keyboard.GetPressedKeys()[0]);
                    }
                    if (wasKeyPressed(Keys.Enter))
                    {
                        if (role == Role.Encoder)
                            game = new EncodingGame(difficulty);
                        else
                            game = new DecodingGame(difficulty);
                        
                        menuItemCounter = 0;
                        gameState = GameState.Playing;
                    }
                    break;
                case GameState.Instructions:
                    break;
                case GameState.Playing:
                    game.Update(keyboard, oldKeyboard);
                    break;
                case GameState.GameOver:
                    break;
            }

            base.Update(gameTime);
        }
        // Changes selected menu item based on key pressed.
        private void switchMenuItem(Keys key, int numMenuItems)
        {
            switch (key)
            {
                case Keys.Up:
                    menuItemCounter = (menuItemCounter - 1) % numMenuItems;
                    break;
                case Keys.Down:
                    menuItemCounter = (menuItemCounter + 1) % numMenuItems;
                    break;
            }
            
        }
        // Changes selected role based on key pressed.
        private void switchChosenRole(Keys key) 
        {
            switch(key) 
            {
                case Keys.Right:
                    role = (Role)((int)role == NUM_ROLES - 1 ? 0 : (int)role + 1);
                    break;
                case Keys.Left:
                    role = (Role)((int)role == 0 ? NUM_ROLES - 1 : (int) role - 1);
                    break;
            }
        }

        // Changes selected difficulty based on key pressed.
        private void switchChosenDifficulty(Keys key) 
        {
            switch(key) 
            {
                case Keys.Right:
                    difficulty = (Difficulty)((int)difficulty == NUM_DIFFICULTIES - 1 ? 0 : (int)difficulty + 1);
                    break;
                case Keys.Left:
                    difficulty = (Difficulty)((int)difficulty == 0 ? NUM_DIFFICULTIES - 1 : (int)difficulty - 1);
                    break;
            }
        }

        // Determines whether a given key was pressed, where a press occurs
        // only once per physical key depression.
        private Boolean wasKeyPressed(Keys key)
        {
            return (keyboard.IsKeyDown(key) && oldKeyboard.IsKeyUp(key));
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(50, 50, 50));

            Vector2 offset = new Vector2(10, 10);

            spriteBatch.Begin();

            switch (gameState)
            {
                case GameState.Intro:
                    drawIntroLogo(spriteBatch);
                    break;
                case GameState.Config:
                    drawRoleSelector(spriteBatch);
                    drawDifficultySelector(spriteBatch);
                    break;
                case GameState.Instructions:
                    game.Draw(spriteBatch, offset);
                    break;
                case GameState.Playing:
                    break;
                case GameState.GameOver:
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void drawIntroLogo(SpriteBatch spriteBatch)
        {
            /*
            // NOTE TO SELF: Doesn't work, width and height round to 0, too tired to figure out why
            // at the moment so hardcoding it for now
            double scaleFactor = (imgLogo.Height / imgLogo.Width) * WINDOW_WIDTH;
            int x = (int)(WINDOW_WIDTH / 2.0 - (imgLogo.Width * scaleFactor / 2.0));
            int y = (int)(WINDOW_HEIGHT / 2.0 + (imgLogo.Height * scaleFactor / 2.0));
            spriteBatch.Draw(imgLogo, new Rectangle(x, y,
                (int)(imgLogo.Width * scaleFactor), (int)(imgLogo.Height * scaleFactor)), Color.White);
            Console.WriteLine("(" + x + "," + y + ")  " + (int)(imgLogo.Width * scaleFactor) + " x " + (int)(imgLogo.Height * scaleFactor));
             */

            spriteBatch.Draw(imgLogo, new Vector2(100, 100), Color.White);
        }

        private void drawRoleSelector(SpriteBatch spriteBatch)
        {
            // NEEDSWORK: Don't hardcode this
            spriteBatch.Draw(imgRoles, new Vector2(50, 50), Color.White);
            spriteBatch.Draw(imgSelector, new Rectangle(100 + (150 * (int)role), 100, 20, 20), Color.SlateGray);
            if (menuItemCounter == 0)
            {
                spriteBatch.Draw(imgSelector, new Rectangle(15, 60, 20, 20), Color.SlateGray);
            }
        }
        private void drawDifficultySelector(SpriteBatch spriteBatch)
        {
            // NEEDSWORK: Don't hardcode this
            spriteBatch.Draw(imgDifficulties, new Vector2(50, 300), Color.White);
            spriteBatch.Draw(imgSelector, new Rectangle(100 + (150 * (int)difficulty), 350, 20, 20), Color.SlateGray);
            if (menuItemCounter != 0)
            {
                spriteBatch.Draw(imgSelector, new Rectangle(15, 310, 20, 20), Color.SlateGray);
            }
        }
    }
}
