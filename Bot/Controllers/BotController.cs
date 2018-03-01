using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Bot.Models;
using Microsoft.Extensions.Options;
using System.Net.Http;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using Bot.Models.Schedulers;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
namespace Bot.Controllers
{
    public abstract class BotController : Controller
    {
        protected HttpClient GetHttpClient { get; } = new HttpClient();
        protected BotConfig GetBotConfig { get; }
        protected static ITelegramBotClient BotClient { get; private set; }
        protected static string CurrentHostUri { get; private set; }

        public BotController(IOptions<BotConfig> config)
        {
            GetBotConfig = config.Value;
            if (BotClient == null)
            {
                BotClient = new TelegramBotClient(config.Value.Token);
            }
        }

        public void SetBotWebhook(string uri)
        {
            CurrentHostUri = uri;
            BotClient.SetWebhookAsync(uri).Wait();
        }

        public abstract void Post([FromBody]Update update);
    }
}