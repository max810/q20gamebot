using Bot.BotExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Models.BotCommands
{
    public class StopCommand : BotCommand
    {
        public async override void Execute(ITelegramBotClient botClient, Message message = null, string args = "")
        {
            var ids = System.IO.File.ReadAllLines("Chats.txt");
            if (ids.Contains(message.Chat.Id.ToString()))
            {
                botClient.DeleteChat(message.Chat.Id);
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "Password update notifications disabled.");
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "You are not being notified yet."
                    + Environment.NewLine +
                    "To stop receiving notifications you first have to start doing it, right?");
            }
        }
    }
}
