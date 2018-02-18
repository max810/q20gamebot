using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Logic
{
    public static class TelegramBotClientExtensions
    {
        public static async void NotifyUsers(this ITelegramBotClient botClient, string newPassword)
        {
            foreach (var line in System.IO.File.ReadAllLines("Chats.txt"))
            {
                long chatId = Convert.ToInt64(line);
                await botClient.SendTextMessageAsync(chatId, "Password changed! New passport:");
                await botClient.SendTextMessageAsync(chatId, $"`{newPassword}`");
            }
        }

        public async static void ProcessInput(this ITelegramBotClient botClient, Update update)
        {
            if (update == null) return;
            var message = update.Message;
            if (message?.Type == MessageType.TextMessage)
            {
                if (TryParseCommand(message.Text, out string command, out string args))
                {
                    //botClient.ProcessCommand()
                }
                await botClient.SendTextMessageAsync(message.Chat.Id, message.Text);
            }
        }
        
        private static bool TryParseCommand(string message, out string resultCommand, out string args)
        {
            if (!string.IsNullOrWhiteSpace(message)
                && message.StartsWith('/'))
            {
                int index = message.IndexOf(' ');
                if(index == -1)
                {
                    resultCommand = message;
                    args = "";
                }
                else
                {
                    resultCommand = message.Substring(0, index);
                    args = message.Substring(index + 1);
                }
                return true;
            }
            resultCommand = null;
            args = null;
            return false;
        }
    }
}
