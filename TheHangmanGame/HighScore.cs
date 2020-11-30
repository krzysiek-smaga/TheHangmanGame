using System;

namespace TheHangmanGame
{
    public class HighScore
    {
        public string PlayerName
        { get; set; }

        public string Date
        { get; set; }

        public double GuessingTime
        { get; set; }

        public int GuessesCount
        { get; set; }

        public string CapitalToGuess
        { get; set; }

        public HighScore(string playerName, string date, double guessingTime, int guessesCount, string capitalToGuess)
        {
            PlayerName = playerName;
            Date = date;
            GuessingTime = guessingTime;
            GuessesCount = guessesCount;
            CapitalToGuess = capitalToGuess;
        }

        public static HighScore FromHighScore(string line)
        {
            string[] values = line.Split('|');
            HighScore highScoreObject = new HighScore(
                Convert.ToString(values[0]),
                Convert.ToString(values[1]),
                Convert.ToDouble(values[2]),
                Convert.ToInt32(values[3]),
                Convert.ToString(values[4])
                );
            return highScoreObject;
        }
    }
}
