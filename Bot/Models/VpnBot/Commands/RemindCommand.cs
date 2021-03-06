﻿using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Models.VpnBot.Commands
{ 
    public class RemindCommand: BotCommand
    {
        public async override void Execute(ITelegramBotClient botClient, Message message = null, string args = "")
        {
            string pwd;
            using(var stream = new StreamReader("Files/Password.txt"))
            {
                pwd = await stream.ReadLineAsync();
            }
            await botClient.SendTextMessageAsync(message.Chat.Id, "Current password is:");
            await botClient.SendTextMessageAsync(message.Chat.Id, $"`{pwd}`", ParseMode.Markdown);
        }
    }
}
