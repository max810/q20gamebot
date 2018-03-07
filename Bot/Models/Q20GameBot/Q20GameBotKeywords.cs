using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Models.Q20GameBot
{
    public static class Q20GameBotKeywords
    {
        public static IEnumerable<string> AnswerKeywords =>
            new string[] { "Yes", "No", "Usually", "Sometimes", "Doubtful", "Probably", "Maybe", "Unknown" };

        public static IEnumerable<string> CategoryKeywords =>
            new string[] { "Animal", "Vegetable", "Mineral", "Concept", "Unknown" };

        public static IEnumerable<string> GuessKeywoards =>
            new string[] { "Right", "Wrong" };

        public static IEnumerable<string> StartKeywords => 
            new string[] { "Start", "Play again", "/start" };

        public static string ExitKeywoard => "EXIT";
        public static string GameFinishKeywords => "Play again";
    }
}
