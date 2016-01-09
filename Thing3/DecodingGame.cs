using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thing3
{
    // User decodes a code designed by the computer.
    class DecodingGame : MastermindGame
    {
        public DecodingGame()
        {
            isCodeCracked = false;
            numTurns = 0;
            currPlayer = PlayerTurn.Computer;
        }

        override public void LoadContent(ContentManager content)
        {
            
        }

        override public void Update()
        {

        }

        override public void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {

        }
    }
}
