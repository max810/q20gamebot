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
using Bot.VpnBotExtensions;
using System.IO;
using Bot.Models.Schedulers;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Bot.Controllers
{
    //[Produces("application/json")]
    [Route("vpnbot")]
    public class VpnBotController : BotController
    {
        private PasswordUpdateHostedService passwordUpdateService;

        public VpnBotController(IOptions<BotConfig> botConfig, PasswordUpdateHostedService service) : base(botConfig)
        {
            passwordUpdateService = service;
        }

        [Route("start")]
        public string Start()
        {
            string result = "";
            if (CurrentHostUri == null)
            {
                var host = Request.Host;
                string uri = host.ToString();
                if (!Request.IsHttps)
                {
                    uri = "https://" + uri;
                }
                uri += "/vpnbot/update";
                SetBotWebhook(uri);
                result = "bot launched";
            }
            else
            {
                result = "bot is working";
            }

            if (passwordUpdateService.PasswordProcessorDelegate == null)
            {
                passwordUpdateService.PasswordProcessorDelegate = (pwd =>
                {
                    BotClient.NotifyUsers(pwd);
                    using (var client = new HttpClient())
                    {
                        using (var message = new HttpRequestMessage(HttpMethod.Get, CurrentHostUri))
                        {
                            client.SendAsync(message).Wait();
                        }
                    }
                });
                passwordUpdateService.StartAsync(CancellationToken.None);
            }

            return result;
        }

        [Route("update")]
        [HttpPost]
        public override void Post([FromBody]Update update)
        {
            BotClient.ProcessInput(update);
        }
    }
}