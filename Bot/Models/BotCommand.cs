using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Models
{
    public abstract class BotCommand
    {
        public abstract void Execute(ITelegramBotClient botClient, Message message = null, string args = "");
    }
}
