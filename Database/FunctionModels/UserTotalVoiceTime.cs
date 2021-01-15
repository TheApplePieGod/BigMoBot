using System;
using System.Collections.Generic;
using System.Text;

namespace BigMoBot.Database.FunctionModels
{
    public class UserTotalVoiceTime
    {
        public string UserName { get; set; }
        public int TotalSecondsInVoice { get; set; }
    }
}
