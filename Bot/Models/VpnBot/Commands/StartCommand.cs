using Bot.VpnBotExtensions;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Models.VpnBot.Commands
{
    public class StartCommand : BotCommand
    {
        public async override void Execute(ITelegramBotClient botClient, Message message = null, string args = "")
        {
            var ids = System.IO.File.ReadAllLines("Files/Chats.txt");
            if (ids.Contains(message.Chat.Id.ToString()))
            {
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "You're already being notified.");
            }
            else
            {
                botClient.AddChat(message.Chat.Id);
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "From now on you will be notified each time a vpnbook password changes.");
            }
        }
    }
}
