using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using hangmanLib;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Activity = Microsoft.Bot.Connector.Activity;

namespace hangmanBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private static async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            StateClient stateClient = activity.GetStateClient();
            BotState botState = new BotState(stateClient);
            BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);

            List<string> endGame = new List<string>{"END", "STOP", "QUIT"};

            HangMan hang = new HangMan();

            if (userData.GetProperty<GameCard>("GameData") != null)
            {
                hang.Game = userData.GetProperty<GameCard>("GameData");
            }
            else
            {
                var data = context.UserData;
                data.SetValue("GameData", hang.Game);
                //await botState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
            }

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            if (activity.Text.ToUpper().Contains("PLAY"))
            {
                hang = new HangMan();
                hang.GetWord();

                var data = context.UserData;
                data.SetValue("GameData", hang.Game);

                // return our reply to the user
                await context.PostAsync($"All right! Let's play Hangman!");
                
                await context.PostAsync($"This word has {hang.Game.Word.Length} letters, start guessing!");
            }

            if (endGame.Contains(activity.Text.ToUpper()))
            {
                var data = context.UserData;
                await context.PostAsync("Game over, thanks for playing");
                data.Clear();

                await context.PostAsync("Say PLAY to play again");
            }

            if (activity.Text.Length == 1)
            {
                int ret = -5;
                if (Char.IsLetter(activity.Text[0]))
                {
                    ret = hang.GuessLetter(activity.Text[0]);
                }

                if (ret == 0)
                {
                    var data = context.UserData;
                    data.SetValue("GameData", hang.Game);

                    if (hang.Game.Status == Status.Lost)
                    {
                        await context.PostAsync($"Sorry, Charlie! You lose! The word was {hang.Game.Word}");

                        data.Clear();

                        await context.PostAsync("Say PLAY to play again");
                    }
                    else
                    {
                        await context.PostAsync($"Sorry, that was an incorrect guess! You have {hang.Game.Lives} body parts left!");
                        await context.PostAsync($"{hang.Pretty()} guess again!");
                    }

                }
                else if (ret == -1)
                {
                    var data = context.UserData;
                    data.SetValue("GameData", hang.Game);

                    await context.PostAsync($"You already guessed that!");

                    await context.PostAsync($"{hang.Pretty()} guess again!");
                }
                else if (ret == 1)
                {
                    var data = context.UserData;
                    data.SetValue("GameData", hang.Game);

                    await context.PostAsync("Great guess!");
                    if (hang.Game.Status == Status.Won)
                    {
                        await context.PostAsync($"{hang.Pretty()}");
                        await context.PostAsync($"Congrats! You win!");

                        data.Clear();

                        await context.PostAsync("Say PLAY to play again");

                    }
                    else
                    {
                        await context.PostAsync($"{hang.Pretty()} guess again!");
                    }
                    
                }

            }
            
            context.Wait(MessageReceivedAsync);
        }

    }
}