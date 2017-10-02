using System.Collections.Generic;

namespace hangmanLib
{
    public enum Status
    {
        InProgress,
        Won,
        Lost
    }
    public class GameCard
    {
        public string Word { get; set; }
        public int Lives { get; set; } = 5;
        public List<char> Guesses { get; set; }  = new List<char>();
        public string CurrentWord { get; set; }
        public Status Status { get; set; } = Status.InProgress;
    }
}
