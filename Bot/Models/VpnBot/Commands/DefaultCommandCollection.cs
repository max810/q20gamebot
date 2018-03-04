using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Bot.Models.VpnBot.Commands
{
    public static class VpnBotCommandCollection
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
        static VpnBotCommandCollection()
        {
            var dict = new Dictionary<string, BotCommand>()
            {
                {"/start", new StartCommand()},
                {"/help", new HelpCommand()},
                {"/stop", new StopCommand()},
                {"/remind", new RemindCommand()}
            };
            allCommands = new ReadOnlyDictionary<string, BotCommand>(dict);
        }
    }
}
