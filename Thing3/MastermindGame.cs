using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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

        protected Boolean isCodeCracked;
        protected int numTurns;
        protected PlayerTurn currPlayer;

        abstract public void LoadContent(ContentManager content);
        // NEEDSWORK: Update probably needs to take something as a parameter
        abstract public void Update();
        abstract public void Draw(SpriteBatch spriteBatch, Vector2 offset);
    }
}
