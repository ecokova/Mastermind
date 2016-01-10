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
        private int numExact;
        private int numAlmost;
        // The code is represented as a sequence of indices of colors in the codeColors array
        private int[] code;             
        private int[] codeColorCounts;
        private int[] playerGuess;
        private int[] playerGuessColorCounts;

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
                
                //give feedback on player's entry
                for (int i = 0; i < CODE_LENGTH; i++)
                {
                    // Same color in same spot
                    if (code[i] == playerGuess[i])
                        numExact++;
                    // Same color, but in other spots
                    else
                    {
                        Console.WriteLine("Color: " + code[i] + ", player has " + playerGuessColorCounts[code[i]] + ", computer has " +
                            codeColorCounts[code[i]]);
                        // NEEDSWORK: Double counts colors. Need to have this next line only happen once for each color
                        // Fix when less tired
                        numAlmost += Math.Min(playerGuessColorCounts[code[i]], codeColorCounts[code[i]]);
                    }
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
                    // NEEDSWORK: omfg why is there no indexof
                    int nextIndex = 0;
                    for (int i = 0; i < playerGuess.Length; i++)
                    {
                        if (playerGuess[i] == -1)
                        {
                            nextIndex = i;
                            break;
                        }
                    }
                    // NEEDSWORK: This will eventually be done with a mouse / clicking. Even so,
                    // the logic should be changed so that a color isn't accepted / added to the
                    // code until the user approves it. Leaving it this way for now.
                    
                    //NEEDSWORK: Find a way to iterate through all the keys in the dictionary
                    for (int i = 0; i < codeKeys.Length; i++)
                    {
                        if (wasKeyPressed(codeKeys[i], keyboard, oldKeyboard))
                        {
                            playerGuess[nextIndex] = codeKeysMap[codeKeys[i]];
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

            for (int i = 0; i < playerGuess.Length; i++)
            {
                if (playerGuess[i] == -1)
                    continue;
                spriteBatch.Draw(imgCodeLight, new Rectangle((int)offset.X + codeImgSize * i,
                    imgFOR_DEBUGGING.Height + 10 + (int)offset.Y, codeImgSize, codeImgSize), codeColors[playerGuess[i]]);
            }
            int feedBackOffset = 0;
            for (int i = 0; i < numExact; i++)
            {
                spriteBatch.Draw(imgFeedbackPeg, new Rectangle((int)offset.X + CODE_LENGTH * codeImgSize + feedBackOffset,
                    imgFOR_DEBUGGING.Height + 10 + (int)offset.Y, feedbackImgSize, feedbackImgSize), COLOR_EXACT);
                feedBackOffset += feedbackImgSize;
            }
            for (int i = 0; i < numAlmost; i++)
            {
                spriteBatch.Draw(imgFeedbackPeg, new Rectangle((int)offset.X + CODE_LENGTH * codeImgSize + feedBackOffset,
                    imgFOR_DEBUGGING.Height + 10 + (int)offset.Y, feedbackImgSize, feedbackImgSize), COLOR_ALMOST);
                feedBackOffset += feedbackImgSize;
            }
        }
    }
}
