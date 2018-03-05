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
        public override async void Post([FromBody] Update update)
        {
            var message = update.Message;
            await ResolveAndExecute(message);
        }

        private async Task ResolveAndExecute(Message message)
        {
            var messageText = new string(message.Text.Where(x => !char.IsSymbol(x)).ToArray()).Trim();

            var currentSession = context.Sessions.FirstOrDefault(x => x.ChatId == message.Chat.Id);

            if (currentSession == null)
            {
                if (string.Equals(messageText, Q20GameBotKeywords.StartKeyword, StringComparison.InvariantCultureIgnoreCase))
                {
                    context.Sessions.Add(new Session()
                    {
                        ChatId = message.Chat.Id,
                        LastRequestMade = DateTime.Now,
                        CurrentAddress = null
                    });
                    await BotClient.SendTextMessageAsync(message.Chat.Id, Q20GameBotMessages.GameStartMessage);
                    await BotClient.SendTextMessageAsync(message.Chat.Id, "Choose category", 
                        replyMarkup: Q20GameBotKeyboards.ChooseCategoryKeyboard);

                    await context.SaveChangesAsync();
                }
                else
                {
                    await BotClient.SendTextMessageAsync(message.Chat.Id, Q20GameBotMessages.NotUnderstoodMessage);
                }


                return;
            }

            if (string.Equals(messageText, Q20GameBotKeywords.ExitKeywoard, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Sessions.Remove(currentSession);
                await BotClient.SendTextMessageAsync(message.Chat.Id,
                    Q20GameBotMessages.ExitMessage);

                await context.SaveChangesAsync();

                return;
            }

            if (DateTime.Now - currentSession.LastRequestMade > TimeSpan.FromMinutes(20))
            {
                context.Sessions.Remove(currentSession);
                await BotClient.SendTextMessageAsync(message.Chat.Id,
                    Q20GameBotMessages.SessionExpiredMessage);

                await context.SaveChangesAsync();

                return;
            }

            HtmlDocument currentPage;
            Q20GameState currentState;

            if(currentSession.CurrentAddress == null)
            {
                currentPage = await Q20GameBotTools.GetNewGamePageAsync();
                currentState = Q20GameState.CategoryChoice;
            }
            else
            {
                currentPage = await Q20GameBotTools.GetGamePageAsync(currentSession.CurrentAddress);
                currentState = Q20GameBotTools.ResolveGameState(currentPage);
            }

            try
            {
                Q20GameBotTools.EnsureCategoryCorrespondance(currentState, messageText);
            }
            catch (ArgumentException)
            {
                await BotClient.SendTextMessageAsync(message.Chat.Id, Q20GameBotMessages.NotUnderstoodMessage);
            }

            string nextUri = Q20GameBotTools.GetNextUri(currentPage, messageText);
            currentSession.CurrentAddress = nextUri;
            var nextGamePage = await Q20GameBotTools.GetGamePageAsync(nextUri);
            var nextGamePageState = Q20GameBotTools.ResolveGameState(nextGamePage);

            string replyMessage = Q20GameBotTools.GetMessage(nextGamePage, nextGamePageState);

            await BotClient.SendTextMessageAsync(message.Chat.Id, replyMessage, 
                replyMarkup: Correspondance.CorrespondingKeyboards[nextGamePageState]);
            currentSession.LastRequestMade = DateTime.Now;

            await context.SaveChangesAsync();
        }

    }
}