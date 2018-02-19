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
        private static IReadOnlyDictionary<string, BotCommand> allCommands;
        public static IReadOnlyDictionary<string, BotCommand> GetAllCommands()
        {
            return allCommands;
        }
        public static bool Includes(string command)
        {
            return allCommands.ContainsKey(command);
        }
        static DefaultCommandCollection()
        {
            var dict = new Dictionary<string, BotCommand>()
            {
                {"/start", new StartCommand()},
                {"/help", new HelpCommand()},
                {"/stop", new StopCommand()}
            };
            allCommands = new ReadOnlyDictionary<string, BotCommand>(dict);
        }
    }
}
