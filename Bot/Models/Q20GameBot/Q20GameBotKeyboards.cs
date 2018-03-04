using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Models.Q20GameBot
{
    public static class Q20GameBotKeyboards
    {
        public static ReplyKeyboardMarkup AnswerKeyboard =>
            //new ReplyKeyboardMarkup(new KeyboardButton[4][]
            //{
            //    new KeyboardButton[3] { "Yes", "Maybe", "No" },
            //    new KeyboardButton[3] { "Usually", "Sometimes", "Rarely" },
            //    new KeyboardButton[3] { "Probably", "Depends", "Doubtful" },
            //    new KeyboardButton[3] { "Partly", "Irrelevant", "Unknown" }
            //});
            new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
                new KeyboardButton[] { "✅ Yes", "❌ No" },
                new KeyboardButton[] { "Usually", "Sometimes", "Doubtful" },
                new KeyboardButton[] { "📊 Probably", "Maybe", "❔ Unknown" },
                new KeyboardButton[] { "🚪 EXIT" }
            });

        public static ReplyKeyboardMarkup ChooseCategoryKeyboard =>
            new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
                new KeyboardButton[] { "Animal", "Vegetable", "Mineral" },
                new KeyboardButton[] { "Concept", "Unknown" },
                new KeyboardButton[] { "🚪 EXIT" }
            });

        public static ReplyKeyboardMarkup GuessKeyboard =>
            new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
                new KeyboardButton[] { "Right", "Wrong" },
                new KeyboardButton[] { "🚪 EXIT" }
            });

    }
}
