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
using Hangfire;
using System.IO;

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
        private readonly TelegramBotClient botClient;
        private event Action<string> Notify;

        public MainController(IOptions<BotConfig> botConfig)
        {
            //old connectionstring Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Hangfire;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False

            config = botConfig.Value;
            botClient = new TelegramBotClient(config.Token);
            botClient.SetWebhookAsync("https://26c4ee3f.ngrok.io/bot/update").Wait();
            //using (var server = new BackgroundJobServer())
            //{
            RecurringJob.AddOrUpdate(() => UpdatePassword(), Cron.Minutely);
            //RecurringJob.AddOrUpdate(() => botClient.NotifyUsers("testRecurring"), Cron.Minutely);
            //}
            //botClient.NotifyUsers("test").Wait();
            Notify += (pwd) => botClient.NotifyUsers(pwd);
        }

        [Route("start")]
        [HttpGet]
        public string StartBot()
        {
            return "bot started";
        }

        [Route("update")]
        [HttpPost]
        public void Post([FromBody]Update update)
        {
            botClient.ProcessInput(update);
        }

        public async Task UpdatePassword()
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
                    var newPassword = ParsePassword(document);
                    string oldPassword;
                    using(var stream = new StreamReader("Password.txt"))
                    {
                        oldPassword = await stream.ReadLineAsync();
                    }
                    if (oldPassword == newPassword)
                    {
                        Notify("fail");
                    }
                    else
                    {
                        using (var stream = new StreamWriter("Password.txt", append: false))
                        {
                            stream.WriteLine(newPassword);
                        }
                        Notify(newPassword);
                    }
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