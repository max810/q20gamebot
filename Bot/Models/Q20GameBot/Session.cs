using System;
using System.Collections.Generic;

namespace Bot.Q20GameBot.Models
{
    public partial class Session
    {
        public long ChatId { get; set; }
        public DateTime LastRequestMade { get; set; }
    }
}
