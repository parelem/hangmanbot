using RestSharp;

namespace hangmanLib
{
    public class HangMan
    {
        public GameCard Game { get; set; }

        public HangMan()
        {
            Start();
        }

        private void Start()
        {
            GameCard newGame = new GameCard();
           // newGame.Word = GetWord();
            //newGame.CurrentWord = new string('_', newGame.Word.Length);
            Game = newGame;
        }

        public bool GetWord()
        {
            bool retVal = false;
            
            RestClient client = new RestClient("http://randomword.setgetgo.com/get.php");
            RestRequest request = new RestRequest {Method = Method.GET};
            IRestResponse response = client.Execute(request);
            var word = response.Content;
            Game.Word = word.ToUpper();
            Game.CurrentWord = new string('@', Game.Word.Length);
            
            return retVal;
        }

        public string Pretty()
        {
            //return Game.CurrentWord.Aggregate("", (current, c) => current + (c + " "));
            return Game.CurrentWord;
        }

        public int GuessLetter(char letter)
        {
            int correct = -1;

            string tempLetter = letter.ToString().ToUpper();

            if (Game.Guesses.Contains(letter))
            {
                return correct;
            }
            Game.Guesses.Add(letter);

            if (Game.Word.Contains(tempLetter))
            {
                correct = 1;

                for (int i = 0; i < Game.Word.Length; i++)
                {
                    if (Game.Word[i].ToString().ToUpper().Equals(tempLetter))
                    {
                        Game.CurrentWord = Game.CurrentWord.Remove(i, 1).Insert(i, tempLetter);
                    }    
                }

                if (Game.CurrentWord.Equals(Game.Word))
                {
                    Game.Status = Status.Won;
                }
            }
            else
            {
                correct = 0;
                Game.Lives --;
                if (Game.Lives == 0)
                {
                    Game.Status = Status.Lost;
                }
            }

            return correct;
        }
    }
}
