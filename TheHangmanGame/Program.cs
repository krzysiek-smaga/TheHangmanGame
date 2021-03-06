﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace TheHangmanGame
{
    class Program
    {
        public static void Main(string[] args)
        {
            
            // FILE PATH to EUROPEAN countries and capitals
            string textFilePath1 = @"..\..\european_countries_and_capitals.txt";
            // FILE PATH to countries_and_capitals.txt
            string textFilePath2 = @"..\..\countries_and_capitals.txt";
            // FILE PATH to high_score.txt
            string textFilePath3 = @"..\..\high_score.txt";

            // Import capitals and countries from attached file
            List<string> countries = new List<string>();
            List<string> capitals = new List<string>();
            foreach (string line in File.ReadAllLines(textFilePath2))
            {
                string[] words = line.Split('|');
                countries.Add(words[0].Trim());
                capitals.Add(words[1].Trim());
            }

            // Declaring some necessary variables
            bool won = false;
            string title = "The Hangman Game"; 
            string playAgain = "";
            string alphabet = "abcdefghijklmnoprqstuwvxyz";
            alphabet = alphabet.ToUpper();

            do
            {
                // Measuring time
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                // Guessing count
                int guessesCount = 0;

                int playerLifes = 6;
                List<string> notInWord = new List<string>();

                // Pick one random capital-country pair from list. Country for hint message.
                Random rnd = new Random();
                int randomIndex = rnd.Next(capitals.Count);
                string capitalToGuess = capitals[randomIndex].ToUpper();
                string hint = $"The capital of {countries[randomIndex]}";

                // static capital for tests
                //capitalToGuess = "WARSAW";

                char[] charArr = capitalToGuess.ToCharArray();
                string showGuessedWord = String.Join(" ", charArr);

                // Making visualization in dashes of capital tu guess
                string[] dashes = new string[capitalToGuess.Length];
                for (int i = 0; i < capitalToGuess.Length; i++)
                {
                    switch (capitalToGuess[i])
                    {
                        case '-':
                            dashes[i] = "-";
                            break;
                        case ' ':
                            dashes[i] = " ";
                            break;
                        default:
                            dashes[i] = "_";
                            break;
                    }
                }

                do
                {
                    //Print title screen, capital in dashes to guess, lifes, hint if one life left
                    TitleScreen(title, playerLifes, notInWord, hint, dashes);

                    //Ask if he/she woould like to guess letter or whole word
                    //Do loop while he/she input correct letter
                    string choose = "";
                    do
                    {
                        Console.WriteLine("Would You like to guess a [L]etter or whole [W]ord(s)?");
                        Console.WriteLine("Type 'L' or 'W' and press Enter");
                        string playerChoose = Console.ReadLine();
                        choose = Convert.ToString(playerChoose).ToUpper();
                    } while (choose != "L" && choose != "W");


                    if (choose == "L") // when player chose to type letter
                    {
                        string playerGuessLetter = "";
                        do
                        {
                            Console.WriteLine("Type a letter and press Enter");
                            string playerInputLetter = Console.ReadLine();
                            playerGuessLetter = Convert.ToString(playerInputLetter).ToUpper();
                        } while (!(playerGuessLetter.Length == 1 && alphabet.Contains(playerGuessLetter)));

                        if (capitalToGuess.Contains(playerGuessLetter))
                        {
                            //exchange letters in dashes
                            for (int i = 0; i < capitalToGuess.Length; i++)
                            {
                                if (playerGuessLetter[0] == capitalToGuess[i])
                                {
                                    dashes[i] = playerGuessLetter;
                                }
                            }
                            //compare dashes with capital to guess
                            if (String.Join("", dashes) == capitalToGuess)
                            {
                                won = true;
                            }
                        }
                        else
                        {
                            //add letter to not-in-word list
                            notInWord.Add(playerGuessLetter);
                            //take one life
                            playerLifes--;
                        }
                    }
                    else // when player chose to type whole word(s)
                    {
                        Console.WriteLine("Type a word(s) and press Enter");
                        string playerInputWord = Console.ReadLine();
                        string playerGuessWord = Convert.ToString(playerInputWord).ToUpper();

                        if (playerGuessWord == capitalToGuess)
                        {
                            // if player guessed capital correct then change won to true
                            won = true;
                        }
                        else
                        {
                            // take two lifes
                            playerLifes -= 2;
                        }
                    }

                    guessesCount++;
                    //Console.ReadLine();
                } while (!won && playerLifes > 0);

                stopwatch.Stop();
                TimeSpan timeSpan = stopwatch.Elapsed;

                Console.Clear();
                Console.WriteLine(title);
                HangTheMan(playerLifes);
                Console.WriteLine();
                Console.WriteLine(" " + showGuessedWord);
                Console.WriteLine();

                if (won)
                {
                    double seconds = Math.Round(timeSpan.TotalSeconds, 2);
                    Console.WriteLine("Congrats! You Won!");
                    Console.WriteLine($"You guessed the capital after {guessesCount} letters. It took you {seconds} seconds.");
                    Console.WriteLine("");

                    // Variables for High score
                    DateTime localDate = DateTime.Now;
                    string dateTimeNow = localDate.ToString();

                    Console.WriteLine("Type your name and press Enter");
                    string playerInputName = Console.ReadLine();
                    string playerName = Convert.ToString(playerInputName).ToUpper();

                    string highScoreLine = $"{playerName}|{dateTimeNow}|{seconds}|{guessesCount}|{capitalToGuess}";
                    // Saving score to a file
                    File.AppendAllText(textFilePath3, highScoreLine + Environment.NewLine);

                }
                else
                {
                    Console.WriteLine("You lost! Better luck next time!");
                }
                Console.WriteLine();
                won = false;

                List<HighScore> highScores = File.ReadAllLines(textFilePath3)
                        .Select(line => HighScore.FromHighScore(line))
                        .ToList();

                var newHigh = highScores.OrderBy(x => x.GuessingTime);
                File.WriteAllText(textFilePath3, String.Empty);

                Console.WriteLine("Top 10 Players: ");
                Console.WriteLine();

                int j = 0;
                foreach (HighScore score in newHigh)
                {
                    string highScoreLine = $"{score.PlayerName}|{score.Date}|{score.GuessingTime}|{score.GuessesCount}|{score.CapitalToGuess}";
                    if (j < 10)
                    {
                        Console.WriteLine((j + 1) + ". " + highScoreLine);
                        File.AppendAllText(textFilePath3, highScoreLine + Environment.NewLine);
                    }
                    else
                    {
                        break;
                    }
                    j++;
                }
                Console.WriteLine();

                //Question about restarting game
                playAgain = RestartingGame();

            } while (playAgain == "Y");

            Console.WriteLine("End of Program. Press any key to Quit");
            Console.ReadLine();
        }

        private static string RestartingGame()
        {
            string playAgain;
            Console.WriteLine("Would You like to play again?");
            Console.WriteLine("Type 'Y' to try again or press any key to quit.");
            string playerChoose2 = Console.ReadLine();
            playAgain = Convert.ToString(playerChoose2).ToUpper();
            return playAgain;
        }

        private static void TitleScreen(string title, int playerLifes, List<string> notInWord, string hint, string[] dashes)
        {
            Console.Clear();
            Console.WriteLine(title);
            HangTheMan(playerLifes);  // Method prints ASCII art
            Console.WriteLine();
            Console.WriteLine(" " + String.Join(" ", dashes));
            Console.WriteLine();
            Console.WriteLine($"Life: {playerLifes}");
            if (notInWord.Count > 0)
            {
                Console.WriteLine($"Wrong letters: {String.Join(" ", notInWord)}");
            }
            if (playerLifes == 1)
            {
                Console.WriteLine($"Hint: {hint}");
            }
            Console.WriteLine();
        }

        public static void HangTheMan(int lifes)
        {
            switch (lifes)
            {
                case 6:
                    Console.WriteLine("     +---+");
                    Console.WriteLine("     |   |");
                    Console.WriteLine("         |");
                    Console.WriteLine("         |");
                    Console.WriteLine("         |");
                    Console.WriteLine("         |");
                    Console.WriteLine("   =========");
                    break;
                case 5:
                    Console.WriteLine("     +---+");
                    Console.WriteLine("     |   |");
                    Console.WriteLine("     O   |");
                    Console.WriteLine("         |");
                    Console.WriteLine("         |");
                    Console.WriteLine("         |");
                    Console.WriteLine("   =========");
                    break;
                case 4:
                    Console.WriteLine("     +---+");
                    Console.WriteLine("     |   |");
                    Console.WriteLine("     O   |");
                    Console.WriteLine("     |   |");
                    Console.WriteLine("         |");
                    Console.WriteLine("         |");
                    Console.WriteLine("   =========");
                    break;
                case 3:
                    Console.WriteLine("     +---+");
                    Console.WriteLine("     |   |");
                    Console.WriteLine("     O   |");
                    Console.WriteLine("    /|   |");
                    Console.WriteLine("         |");
                    Console.WriteLine("         |");
                    Console.WriteLine("   =========");
                    break;
                case 2:
                    Console.WriteLine("     +---+");
                    Console.WriteLine("     |   |");
                    Console.WriteLine("     O   |");
                    Console.WriteLine("    /|\\  |");
                    Console.WriteLine("         |");
                    Console.WriteLine("         |");
                    Console.WriteLine("   =========");
                    break;
                case 1:
                    Console.WriteLine("     +---+");
                    Console.WriteLine("     |   |");
                    Console.WriteLine("     O   |");
                    Console.WriteLine("    /|\\  |");
                    Console.WriteLine("    /    |");
                    Console.WriteLine("         |");
                    Console.WriteLine("   =========");
                    break;
                default:
                    Console.WriteLine("     +---+");
                    Console.WriteLine("     |   |");
                    Console.WriteLine("     O   |");
                    Console.WriteLine("    /|\\  |");
                    Console.WriteLine("    / \\  |");
                    Console.WriteLine("         |");
                    Console.WriteLine("   =========");
                    break;

            }
        }
    }
}
