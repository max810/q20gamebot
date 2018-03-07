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
        private static Dictionary<long, HtmlDocument> CurrentPages = new Dictionary<long, HtmlDocument>();
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
                uri += "/q20gamebot/update";
                SetBotWebhook(uri);
                return "bot launched";
            }
            return "bot is working";
        }

        [Route("update")]
        public async Task Post([FromBody] Update update)
        {
            var message = update.Message;
            if (message.Type == MessageType.TextMessage)
            {
                await ResolveAndExecute(message);
            }
        }

        private async Task ResolveAndExecute(Message message)
        {
            //A-z
            var messageText = new string(message.Text.Where(x => x <= 122 && x >= 32).ToArray()).Trim();

            var currentSession = context.Sessions.FirstOrDefault(x => x.ChatId == message.Chat.Id);

            if (currentSession == null)
            {
                if (Q20GameBotKeywords.StartKeywords.Contains(messageText))
                {
                    context.Sessions.Add(new Session()
                    {
                        ChatId = message.Chat.Id,
                        LastRequestMade = DateTime.Now,
                    });

                    CurrentPages[message.Chat.Id] = await Q20GameBotTools.GetNewGamePageAsync();

                    await context.SaveChangesAsync();

                    await BotClient.SendTextMessageAsync(message.Chat.Id, Q20GameBotMessages.GameStartMessage);
                    await BotClient.SendTextMessageAsync(message.Chat.Id, "**Q1.  Is it classified as Animal, Vegetable or Mineral?**",
                        replyMarkup: Q20GameBotKeyboards.ChooseCategoryKeyboard, parseMode: ParseMode.Markdown);

                }
                else
                {
                    await BotClient.SendTextMessageAsync(message.Chat.Id, "Wrong message - shoul've been Start");
                }


                return;
            }

            if (string.Equals(messageText, Q20GameBotKeywords.ExitKeywoard, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Sessions.Remove(currentSession);
                await context.SaveChangesAsync();
                await BotClient.SendTextMessageAsync(message.Chat.Id,
                    Q20GameBotMessages.ExitMessage);

                await BotClient.SendTextMessageAsync(message.Chat.Id,
                    Q20GameBotMessages.MainMenuMessage,
                    replyMarkup: Q20GameBotKeyboards.MainMenuKeyboard);

                return;
            }

            if (DateTime.Now - currentSession.LastRequestMade > TimeSpan.FromMinutes(5))
            {
                context.Sessions.Remove(currentSession);
                await BotClient.SendTextMessageAsync(message.Chat.Id,
                    Q20GameBotMessages.SessionExpiredMessage);

                await BotClient.SendTextMessageAsync(message.Chat.Id,
                    Q20GameBotMessages.MainMenuMessage,
                    replyMarkup: Q20GameBotKeyboards.MainMenuKeyboard);

                await context.SaveChangesAsync();

                return;
            }

            HtmlDocument currentPage = CurrentPages[currentSession.ChatId];
            Q20GameState currentState = Q20GameBotTools.ResolveGameState(currentPage);
            try
            {
                Q20GameBotTools.EnsureCategoryCorrespondance(currentState, messageText);
            }
            catch (ArgumentException)
            {
                await BotClient.SendTextMessageAsync(message.Chat.Id, Q20GameBotMessages.NotUnderstoodMessage);
                return;
            }

            string nextUri = Q20GameBotTools.GetNextUri(currentPage, messageText);
            var nextGamePage = await Q20GameBotTools.GetGamePageAsync(nextUri);
            var nextGamePageState = Q20GameBotTools.ResolveGameState(nextGamePage);

            CurrentPages[currentSession.ChatId] = nextGamePage;

            string question = HtmlEntity.DeEntitize(Q20GameBotTools.GetQuestion(nextGamePage)).Trim();

            await BotClient.SendTextMessageAsync(message.Chat.Id, question, 
                replyMarkup: Correspondance.CorrespondingKeyboards[nextGamePageState]);

            if (nextGamePageState == Q20GameState.GameFinish)
            {
                context.Sessions.Remove(currentSession);
            }
            else
            {
                currentSession.LastRequestMade = DateTime.Now;
            }

            await context.SaveChangesAsync();
        }
    }
}