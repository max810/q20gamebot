using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Models.BotCommands
{
    public class HelpCommand : BotCommand
    {
        public async override void Execute(ITelegramBotClient botClient, Message message = null, string args = "")
        {
            string reply;
            if (string.IsNullOrWhiteSpace(args))
            {
                reply =
@"Hi! This bot will notify you about the password changes at vpnbook.com.
You no longer need to check the web-site every time you want to use VPN.
Here's the list of all commands:

/help : display help message
/help <command> : get info about specified command
/start : start receiving notifications
/stop : stop receiving notifications
/remind : get the current password";

            }
            else
            {
                if (!args.StartsWith('/'))
                {
                    args = "/" + args;
                }
                if (DefaultCommandCollection.Includes(args))
                {
                    reply =
                        $"Info about command \"{args}\"{Environment.NewLine + Environment.NewLine}"
                        + commandDescriptions[args];
                }
                else
                {
                    reply =
    $"Sorry, no such command: \"{args}\"";
                }
            }
            await botClient.SendTextMessageAsync(message.Chat.Id, reply);
        }
        private static Dictionary<string, string> commandDescriptions = new Dictionary<string, string>()
        {
            {"/help", "Shows help message."},
            {"/start", "Start receiving password update notifications."
                + Environment.NewLine +
                "Each time the password at vpnbook.com is updated, this bot will send you a message with a new password."},
            {"/stop", "Stop receiving password update notifications. This bot will no longer send you any messages."
            + Environment.NewLine +
                "You can start receiving them again by simply typing /start command."},
            {"/remind", "Get the current password (in casw you have forgotten it)."}
        };
    }
}
