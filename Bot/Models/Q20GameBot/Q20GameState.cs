using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Models.Q20GameBot
{
    public enum Q20GameState
    {
        CategoryChoice,
        AnswerPending,
        GuessConfirmation,
        GameFinish
    }
}
