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
    // User decodes a code designed by the computer.
    class DecodingGame : MastermindGame
    {
        private struct Guess
        {
            public int[] code;
            public int numExact;
            public int numAlmost;
        }
        private int numExact;
        private int numAlmost;
        // The code is represented as a sequence of indices of colors in the codeColors array
        private int[] code;             
        private int[] codeColorCounts;
        private int[] playerGuess;
        private int[] playerGuessColorCounts;

        private List<Guess> guesses;

        private Texture2D imgFOR_DEBUGGING;

        public DecodingGame(Difficulty _difficulty)
        {
            initializeKeysMap();
            isCodeCracked = false;
            numTurns = 0;
            currPlayer = PlayerTurn.User;
            difficulty = _difficulty;

            numAlmost = 0;
            numExact = 0;

            codeImgSize = 20;
            feedbackImgSize = 10;

            Random random = new Random();

            code = new int[CODE_LENGTH];
            codeColorCounts = new int[codeColors.Length];
            for (int i = 0; i < CODE_LENGTH; i++)
            {
                code[i] = random.Next(codeColors.Length);
                codeColorCounts[code[i]]++;
            }

            playerGuess = new int[CODE_LENGTH];
            playerGuessColorCounts = new int[codeColors.Length];
            resetPlayerGuess();

            guesses = new List<Guess>();            
        }

        override public void LoadContent(ContentManager content)
        {
            loadImages(content);
            imgFOR_DEBUGGING = content.Load<Texture2D>("forDebuggingPurposes");
        }

        override public void Update(KeyboardState keyboard, KeyboardState oldKeyboard)
        {
            if (currPlayer == PlayerTurn.Computer)
            {
                Guess currentGuess = new Guess();
                currentGuess.code = new int[playerGuess.Length];
                playerGuess.CopyTo(currentGuess.code, 0);
                currentGuess.numExact = numExact;
                currentGuess.numAlmost = numAlmost;
                guesses.Add(currentGuess);
                // Calculate number of exact
                for (int i = 0; i < CODE_LENGTH; i++)
                {
                    // Same color in same spot
                    if (code[i] == playerGuess[i])
                        numExact++;
                    
                }
                // Calculate number of almost
                numAlmost -= numExact; // Corrects for exact values double counted as "almosts"
                for (int i = 0; i < playerGuessColorCounts.Length; i++)
                {
                    numAlmost += Math.Min(playerGuessColorCounts[i], codeColorCounts[i]);
                }

                if (numExact == CODE_LENGTH)
                    isCodeCracked = true;


                printAnalysis(); // For debugging

                resetPlayerGuess();
                currPlayer = PlayerTurn.User;
            }
            else
            {
                // All slots filled
                if (playerGuess.Count<int>(count => count == -1) == 0)
                {
                    foreach (int color in playerGuess)
                    {
                        playerGuessColorCounts[color]++;
                    }
                    numTurns++;
                    numExact = 0;
                    numAlmost = 0;
                    currPlayer = PlayerTurn.Computer;
                }
                else
                {
                    // First unfilled one
                    int nextIndex = Array.IndexOf(playerGuess, -1);

                    // NEEDSWORK: This will eventually be done with a mouse / clicking. Even so,
                    // the logic should be changed so that a color isn't accepted / added to the
                    // code until the user approves it. Leaving it this way for now.
                    
                    foreach (Keys key in codeKeysMap.Keys)
                    {
                        if (wasKeyPressed(key, keyboard, oldKeyboard))
                        {
                            playerGuess[nextIndex] = codeKeysMap[key];
                            break;
                        }
                    }
                }
            }
        }
        // Determines whether a given key was pressed, where a press occurs
        // only once per physical key depression.
        private Boolean wasKeyPressed(Keys key, KeyboardState keyboard, KeyboardState oldKeyboard)
        {
            return (keyboard.IsKeyDown(key) && oldKeyboard.IsKeyUp(key));
        }

        private void resetPlayerGuess()
        {
            for (int i = 0; i < playerGuess.Length; i++)
            {
                playerGuess[i] = -1;
            }
            for (int i = 0; i < playerGuessColorCounts.Length; i++)
            {
                playerGuessColorCounts[i] = 0;
            }
        }
        private void printAnalysis()
        {
            Console.Write("CODE: ");
            for (int i = 0; i < CODE_LENGTH; i++)
            {
                Console.Write(code[i] + " ");
            }
            Console.Write("\nUSER'S GUESS: ");
            for (int i = 0; i < CODE_LENGTH; i++)
            {
                Console.Write(playerGuess[i] + " ");
            }
            Console.WriteLine("  " + numExact + " exact, " + numAlmost + " almost");
        }
        override public void Draw(SpriteBatch spriteBatch, Vector2 offset)
        {
            spriteBatch.Draw(imgFOR_DEBUGGING, offset, Color.Gray);
            for (int i = 0; i < code.Length; i++ )
            {
                //...........................................width of label      padding   offset         offset due to prev lights
                spriteBatch.Draw(imgCodeLight, new Rectangle(imgFOR_DEBUGGING.Width + 5 + (int)offset.X + codeImgSize * i,
                    (int)offset.Y, codeImgSize, codeImgSize), codeColors[code[i]]);
            }

            drawUserGuesses(spriteBatch, offset);
            // NEEDSWORK: All imgFOR_DEBUGGING references will eventually need to be changed to reflect whatever
            // image I actually put in.
        }

        private void drawUserGuesses(SpriteBatch spriteBatch, Vector2 offset)
        {
            
            foreach (Guess g in guesses) 
            {
                drawSingleUserGuess(g, spriteBatch, offset);
                offset.Y += codeImgSize + 3;
            }
        }

        private void drawSingleUserGuess(Guess guess, SpriteBatch spriteBatch, Vector2 offset)
        {
            // User's guess
            for (int i = 0; i < guess.code.Length; i++)
            {
                if (guess.code[i] == -1)
                    continue;
                Console.Write(guess.code[i]);
                drawCodeLight(spriteBatch, offset, i, codeColors[guess.code[i]]);
            }
            
            int feedBackOffset = 0;

            // Exact feedback markers
            for (int i = 0; i < guess.numExact; i++)
            {
                drawFeedbackPeg(spriteBatch, offset, feedBackOffset, COLOR_EXACT);
                feedBackOffset += feedbackImgSize;
            }
            // Almost feedback markers
            for (int i = 0; i < guess.numAlmost; i++)
            {
                drawFeedbackPeg(spriteBatch, offset, feedBackOffset, COLOR_ALMOST);
                feedBackOffset += feedbackImgSize;
            }
        }

        // Draws a single code light
        private void drawCodeLight(SpriteBatch spriteBatch, Vector2 offset, int lightPos, Color color)
        {
            spriteBatch.Draw(imgCodeLight, new Rectangle((int)offset.X + codeImgSize * lightPos,
                    imgFOR_DEBUGGING.Height + 10 + (int)offset.Y, codeImgSize, codeImgSize), color);
        }
        // Draws a single feedback peg
        private void drawFeedbackPeg(SpriteBatch spriteBatch, Vector2 offset, int feedBackOffset, Color color)
        {
            spriteBatch.Draw(imgFeedbackPeg, new Rectangle((int)offset.X + CODE_LENGTH * codeImgSize + feedBackOffset,
                   imgFOR_DEBUGGING.Height + 10 + (int)offset.Y, feedbackImgSize, feedbackImgSize), color);
        }
    }
}
