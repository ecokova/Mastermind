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
    // User designs a code and the computer tries to crack it.
    class EncodingGame : MastermindGame
    {
        public EncodingGame(Difficulty _difficulty)
        {
            isCodeCracked = false;
            numTurns = 0;
            currPlayer = PlayerTurn.User;
            difficulty = _difficulty;
        }

        override public void LoadContent(ContentManager content)
        {
            
        }

        override public void Update(KeyboardState keyboard, KeyboardState oldKeyboard)
        {

        }

        override public void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {

        }
    }
}
