using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Models.BotCommands
{
    public class StartCommand : IBotCommand
    {
        public void Execute(ITelegramBotClient botClient, Update update = null, string args = "")
        {
            
        }
    }
}
