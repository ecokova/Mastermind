using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thing3
{
    abstract class MastermindGame
    {
        public enum PlayerTurn
        {
            Computer,
            User
        }

        protected Color[] codeColors = {
                                           new Color(230, 30, 30),  // Red
                                           new Color(30, 230, 30),  // Green
                                           new Color(30, 30, 230),  // Blue
                                           new Color(230, 230, 30), // Yellow
                                           new Color(30, 230, 230), // Teal
                                           new Color(230, 30, 230)  // Purple
                                       };
        protected Keys[] codeKeys = {
                                        Keys.R, Keys.G, Keys.B, Keys.Y, Keys.T, Keys.P
                                    };
        protected Dictionary<Keys, int> codeKeysMap;
        protected const int CODE_LENGTH = 4;
        protected Color COLOR_EXACT = new Color(30, 30, 30);
        protected Color COLOR_ALMOST = new Color(255, 255, 255);

        protected Boolean isCodeCracked;
        protected int numTurns;
        protected PlayerTurn currPlayer;
        protected Difficulty difficulty;

        protected Texture2D imgCodeLight;
        protected Texture2D imgFeedbackPeg;
        protected int codeImgSize;
        protected int feedbackImgSize;

        abstract public void LoadContent(ContentManager content);
        // NEEDSWORK: Update probably needs to take something as a parameter
        abstract public void Update(KeyboardState keyboard, KeyboardState oldKeyboard);
        abstract public void Draw(SpriteBatch spriteBatch, Vector2 offset);

        // NEEDSWORK: Find some way to incorporate this into LoadContent?
        protected void loadImages(ContentManager content)
        {
            // NEEDSWORK: Need better images!
            imgCodeLight = content.Load<Texture2D>("SoftOrb");
            imgFeedbackPeg = content.Load<Texture2D>("CodeLight");
        }

        //NEEDSWORK: Find some way to initialize this up above like the arrays
        //NEEDSWORK: Actually, is this even needed for EncodingGame? Or just Decoding? 
        protected void initializeKeysMap()
        {
            codeKeysMap = new Dictionary<Keys, int>();

            for (int i = 0; i < codeKeys.Length; i++)
            {
                codeKeysMap.Add(codeKeys[i], i);
            }
        }
    }
}
