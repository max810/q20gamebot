using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Models.VpnBot.Schedulers
{
    public abstract class HostedService: IHostedService
    {
        private Task currentTask;
        private CancellationTokenSource cts;

        public Task StartAsync(CancellationToken token)
        {
            cts = CancellationTokenSource.CreateLinkedTokenSource(token);

            currentTask = ExecuteAsync(cts.Token);

            return currentTask.IsCompleted ? currentTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken token)
        {
            // Stop called without start
            if (currentTask == null)
            {
                return;
            }

            // Signal cancellation to the executing method
            cts.Cancel();

            // Wait until the task completes or the stop token triggers
            await Task.WhenAny(currentTask, Task.Delay(-1, token));

            // Throw if cancellation triggered
            token.ThrowIfCancellationRequested();
        }

        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
