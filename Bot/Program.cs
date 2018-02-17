using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace Bot
{
    public class Program
    {
        private static string token = "513027360:AAHIgttJtVNz79q22IWdYd-0gUUclu7F9G8";
        public static void Main(string[] args)
        {
            var client = new TelegramBotClient(token);
            client.SetWebhookAsync("https://6b72aed0.ngrok.io/bot/update").Wait();
            BuildWebHost(args).Run();
            //client.SetWebhookAsync().Wait();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
