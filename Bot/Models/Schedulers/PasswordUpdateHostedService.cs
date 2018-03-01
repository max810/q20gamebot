using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Bot.BotExtensions;
using System.Net.Http;
using Bot.Controllers;

namespace Bot.Models.Schedulers
{
    public class PasswordUpdateHostedService : HostedService
    {
        private PasswordUpdateProvider passwordUpdateProvider;
        public PasswordUpdateHostedService(PasswordUpdateProvider provider)
        {
            passwordUpdateProvider = provider;
        }

        public Action<string> PasswordProcessorDelegate { get; set; } = null;

        protected async override Task ExecuteAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                string oldpwd = passwordUpdateProvider.Password;
                await passwordUpdateProvider.UpdatePassword();
                if (passwordUpdateProvider.Password != oldpwd)
                {
                    PasswordProcessorDelegate?.Invoke(passwordUpdateProvider.Password);
                }
                using(var client = new HttpClient())
                {
                    using(var message = new HttpRequestMessage(HttpMethod.Get, MainController.currentHost))
                    {
                        await client.SendAsync(message);
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(10), token);
            }
        }
    }
}
