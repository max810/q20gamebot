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


namespace Bot.Controllers
{
    //[Produces("application/json")]
    [Route("bot")]
    public class MainController : Controller
    {
        private string password = "error: password not set";
        //private DateTimeOffset lastModified = DateTime.UtcNow;
        private string address = "https://www.vpnbook.com";
        private HttpClient httpClient = new HttpClient();

        private BotConfig config;
        private readonly TelegramBotClient botClient;

        public MainController(IOptions<BotConfig> botConfig)
        {
            config = botConfig.Value;
            botClient = new TelegramBotClient(config.Token);
            botClient.SetWebhookAsync("https://26c4ee3f.ngrok.io/bot/update").Wait();
        }

        [Route("update")]
        [HttpPost]
        public void Post([FromBody]Update update)
        {
            botClient.ProcessInput(update);
        }

        private async Task UpdatePassword()
        {
            var requestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(address)
            };
            //requestMessage.Headers.IfModifiedSince = lastModified;

            using (var response = await httpClient.SendAsync(requestMessage))
            {
                if (response.StatusCode == HttpStatusCode.NotModified)
                {
                    return;
                }

                response.EnsureSuccessStatusCode();

                //lastModified = response.Content.Headers.LastModified.Value;
                using (var content = response.Content)
                {
                    string body = await content.ReadAsStringAsync();
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(body);
                    password = ParsePassword(document);
                }
            }
        }

        private string ParsePassword(HtmlDocument document)
        {
            var strongNodes = document.DocumentNode.SelectNodes("//strong");
            string password = strongNodes.Last().InnerText.Split(':').Last().Trim();
            return password;
        }
    }
}