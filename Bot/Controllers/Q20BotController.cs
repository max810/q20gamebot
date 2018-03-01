using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace Bot.Controllers
{
    [Route("q20bot")]
    public class Q20BotController : BotController
    {
        public Q20BotController(IOptions<BotConfig> botConfig): base(botConfig)
        {

        }
        public override void Post([FromBody] Update update)
        {
            throw new NotImplementedException();
        }
    }
}