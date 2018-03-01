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
        public VpnBotController(IOptions<BotConfig> botConfig, PasswordUpdateHostedService service) : base(botConfig)
        { 
            if (service.PasswordProcessorDelegate == null)
            {
                service.PasswordProcessorDelegate = (async pwd =>
                {
                    BotClient.NotifyUsers(pwd);
                    using (var client = new HttpClient())
                    {
                        using (var message = new HttpRequestMessage(HttpMethod.Get, CurrentHostUri))
                        {
                            await client.SendAsync(message);
                        }
                    }
                });
                service.StartAsync(CancellationToken.None);
            }
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
                uri += "/vpnbot/update";
                uri = "https://c00f1ef0.ngrok.io";
                SetBotWebhook(uri);
                return "bot launched";
            }

            return "bot is working";
        }

        [Route("update")]
        [HttpPost]
        public override void Post([FromBody]Update update)
        {
            BotClient.ProcessInput(update);
        }
    }
}