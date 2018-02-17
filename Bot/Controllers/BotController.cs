using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Configuration;
using Bot.Models;
using Microsoft.Extensions.Options;

namespace Bot.Controllers
{
    //[Produces("application/json")]
    [Route("bot")]
    public class BotController : Controller
    {
        // token, который вернул BotFather
        private BotConfig Config;
        private readonly TelegramBotClient client;

        public BotController(IOptions<BotConfig> botConfig)
        {
            Config = botConfig.Value;
            client = new TelegramBotClient(Config.Token);
        }

        [Route("update")]
        [HttpPost]
        public async void Post([FromBody]Update update)
        {
            if (update == null) return;
            var message = update.Message;
            if (message?.Type == MessageType.TextMessage)
            {
                await client.SendTextMessageAsync(message.Chat.Id, message.Text);
            }
        }

        [HttpGet]
        [Route("tryget")]
        public string TryGet()
        {
            return Config.Token;
        }
    }
}