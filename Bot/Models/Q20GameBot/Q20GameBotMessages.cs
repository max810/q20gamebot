using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Models.Q20GameBot
{
    public static class Q20GameBotMessages
    {
        public static string MainMenuMessage => "Welcome to the game '20 questions'.";
        public static string ExitMessage => "Exiting current game session..."
            + Environment.NewLine
            + "Done.";
        public static string NotUnderstoodMessage => "Sorry, i don't understand you.";
        public static string SessionExpiredMessage => "Sorry, the session has expired. Returning to main menu...";
        public static string SessionExpiredAgoFormatMessage => "Sorry, the session has expired {0} {1} ago."
            + Environment.NewLine
            + "Returning to main menu...";
        public static string GameStartMessage => "Games started!";
    }
}
