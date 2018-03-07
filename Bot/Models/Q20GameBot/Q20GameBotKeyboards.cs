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

        public static ReplyKeyboardMarkup MainMenuKeyboard =>
            new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
                new KeyboardButton[] { "Start" }
            }, resizeKeyboard: true);

        public static ReplyKeyboardMarkup GameFinishKeyboard =>
            new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
                new KeyboardButton[] { "Play again" },
            }, resizeKeyboard: true);

    }
}
