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
using System.Net.Http;
using HtmlAgilityPack;
using System.IO;
using System.Net;
using Bot.Logic;


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
        }

        [Route("update")]
        [HttpPost]
        public void Post([FromBody]Update update)
        {
            botClient.ProcessInput(update);
        }

        private async void AddChat(long chatId)
        {
            using (var stream = System.IO.File.AppendText("Chats.txt"))
            {
                await stream.WriteLineAsync(chatId.ToString());
            }
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