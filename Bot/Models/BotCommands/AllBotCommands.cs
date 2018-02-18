using Bot.Models.BotCommands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Models.BotCommands
{
    public static class DefaultCommandCollection
    {
        private static IReadOnlyDictionary<string, IBotCommand> allCommands;
        public static IReadOnlyDictionary<string, IBotCommand> GetAllCommands()
        {
            return allCommands;
        }
        public static bool Includes(string command)
        {
            return allCommands.ContainsKey(command);
        }
        static DefaultCommandCollection()
        {
            var dict = new Dictionary<string, IBotCommand>()
            {
                {"/start", new StartCommand()},
            };
            allCommands = new ReadOnlyDictionary<string, IBotCommand>(dict);
        }
    }
}
