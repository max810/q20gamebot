using Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot.Logic
{
    public static class TelegramBotClientCommandExtensions
    {
        //private static Dictionary<string, IBotCommand> commands = new Dictionary<string, IBotCommand>()
        //{
        //    {"s", default(IBotCommand) }
        //};

        public static void ProcessCommand(this ITelegramBotClient botClient, Update update, string command)
        {
            
        }

        public static void ProcessCommand(this ITelegramBotClient botClient, Update update, string command, string args)
        {

        }

        ////enum
        //public static void ProcessCommand(this ITelegramBotClient botClient, Update update, IBotCommand command)
        //{

        //}

        ////enum
        //public static void ProcessCommand(this ITelegramBotClient botClient, Update update, IBotCommand command, string args)
        //{

        //}

    }
}
