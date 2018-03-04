using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Models;
using Bot.Models.BotConfigs;
using Bot.Models.Q20GameBot;
using Bot.Q20GameBot.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Controllers
{
    [Route("q20gamebot")]
    public class Q20BotController : BotController
    {
        private SessionsContext context;

        public Q20BotController(IOptions<Q20GameBotConfig> botConfig, SessionsContext sessionsContext) : base(botConfig)
        {
            context = sessionsContext;
        }

        [Route("start")]
        public string Start()
        {
            if (CurrentHostUri == null)
            {
                var host = Request.Host;
                string uri = host.ToString();
                if (!Request.IsHttps)
                {
                    uri = "https://" + uri;
                }
                uri = "https://2e8b4193.ngrok.io";
                uri += "/q20gamebot/update";
                SetBotWebhook(uri);
                return "bot launched";
            }
            return "bot is working";
        }

        [Route("update")]
        public async override void Post([FromBody] Update update)
        {

            var message = update.Message;
            ResolveAndExecute(message);

            await BotClient.SendTextMessageAsync(message.Chat.Id, "sadfvf",
                replyMarkup: Q20GameBotKeyboards.AnswerKeyboard,
                parseMode: ParseMode.Markdown);
        }

        private async void ResolveAndExecute(Message message)
        {
            var messageText = new string(message.Text.Where(x => !char.IsSymbol(x)).ToArray());

            var currentSession = context.Sessions.FirstOrDefault(x => x.ChatId == message.Chat.Id);

            if (currentSession == null)
            {
                if (string.Equals(messageText, Q20GameBotKeywords.StartKeyword))
                {
                    StartGame();
                    context.Sessions.Add(new Session()
                    {
                        ChatId = message.Chat.Id,
                        LastRequestMade = DateTime.Now,
                        CurrentAddress = GetNewGameAddress()
                    });
                    await BotClient.SendTextMessageAsync(message.Chat.Id, Q20GameBotMessages.GameStartMessage);
                }
                else
                {
                    await BotClient.SendTextMessageAsync(message.Chat.Id, Q20GameBotMessages.NotUnderstoodMessage);
                }


                return;
            }

            if (string.Equals(messageText, Q20GameBotKeywords.ExitKeywoard, StringComparison.InvariantCultureIgnoreCase))
            {
                await BotClient.SendTextMessageAsync(message.Chat.Id,
                    Q20GameBotMessages.ExitMessage);


                return;
            }

            if (DateTime.Now - currentSession.LastRequestMade > TimeSpan.FromMinutes(20))
            {
                await BotClient.SendTextMessageAsync(message.Chat.Id,
                    Q20GameBotMessages.SessionExpiredMessage);


                return;
            }

            var currentPage = await Q20GameBotTools.GetGamePage(currentSession.CurrentAddress);
            var currentState = Q20GameBotTools.ResolveGameState(currentPage);

            try
            {
                Q20GameBotTools.EnsureCategoryCorrespondance(currentState, messageText);
            }
            catch (ArgumentException)
            {
                await BotClient.SendTextMessageAsync(message.Chat.Id, Q20GameBotMessages.NotUnderstoodMessage);
            }

            

            string replyMessage;
            ReplyKeyboardMarkup keyboard;

            string nextUri = Q20GameBotTools.GetNextUri(currentPage, messageText);

            //ResolveCommand(messageText);
            //TODO

            throw new NotImplementedException("Corresponding keyboards, add messages, ask questions");


        }

        private void ResolveCommand(string messageText)
        {
            throw new NotImplementedException();
        }

        private string GetNewGameAddress()
        {
            throw new NotImplementedException();
        }

        private void StartGame()
        {
            throw new NotImplementedException();
        }


    }
}