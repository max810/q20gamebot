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
using Bot.BotExtensions;
using System.IO;
using Bot.Models.Schedulers;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Bot.Controllers
{
    //[Produces("application/json")]
    [Route("bot")]
    public class MainController : Controller
    {
        //private DateTimeOffset lastModified = DateTime.UtcNow;
        private string address = "https://www.vpnbook.com";
        private HttpClient httpClient = new HttpClient();

        private BotConfig config;
        private static TelegramBotClient botClient = null;
        //private static event Action<string> Notify;
        private static bool webhookSet = false;

        public MainController(IOptions<BotConfig> botConfig, PasswordUpdateHostedService service)
        {
            config = botConfig.Value;
            if(botClient == null)
            {
                botClient = new TelegramBotClient(config.Token);
            }
            if (service.PasswordProcessorDelegate == null)
            {
                service.PasswordProcessorDelegate = (pwd => botClient.NotifyUsers(pwd));
                service.StartAsync(CancellationToken.None);
            }
        }

        [Route("start")]
        [HttpGet]
        public string StartBot()
        {
            if (!webhookSet)
            {
                botClient.SetWebhookAsync(Request.Host.ToUriComponent()).Wait();
                //botClient.SetWebhookAsync(@"https://c00f1ef0.ngrok.io/bot/update").Wait();
                webhookSet = true;
                return "bot launched";
            }
            
            return "bot is working";
        }

        [Route("update")]
        [HttpPost]
        public void Post([FromBody]Update update)
        {
            botClient.ProcessInput(update);
        }
    }
}