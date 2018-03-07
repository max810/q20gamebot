using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Models.Q20GameBot
{
    public static class Correspondance
    {
        public static Dictionary<Q20GameState, IEnumerable<string>> CorrespondingCommands =>
            new Dictionary<Q20GameState, IEnumerable<string>>
            {
                {Q20GameState.CategoryChoice, Q20GameBotKeywords.CategoryKeywords },
                {Q20GameState.AnswerPending, Q20GameBotKeywords.AnswerKeywords },
                {Q20GameState.GuessConfirmation, Q20GameBotKeywords.GuessKeywoards },
            };

        public static Dictionary<Q20GameState, ReplyKeyboardMarkup> CorrespondingKeyboards =>
            new Dictionary<Q20GameState, ReplyKeyboardMarkup>
            {
                {Q20GameState.CategoryChoice, Q20GameBotKeyboards.ChooseCategoryKeyboard},
                {Q20GameState.AnswerPending, Q20GameBotKeyboards.AnswerKeyboard },
                {Q20GameState.GuessConfirmation, Q20GameBotKeyboards.GuessKeyboard },
                {Q20GameState.GameFinish, Q20GameBotKeyboards.GameFinishKeyboard }
            };
    }
}
