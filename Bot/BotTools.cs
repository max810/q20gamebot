using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot
{
    public static class BotTools
    {
        public static bool TryParseCommand(string message, out string resultCommand, out string args)
        {
            if (!string.IsNullOrWhiteSpace(message)
                && message.StartsWith('/'))
            {
                int index = message.IndexOf(' ');
                if (index == -1)
                {
                    resultCommand = message;
                    args = "";
                }
                else
                {
                    resultCommand = message.Substring(0, index);
                    args = message.Substring(index + 1);
                }
                return true;
            }
            resultCommand = null;
            args = null;
            return false;
        }
    }
}
