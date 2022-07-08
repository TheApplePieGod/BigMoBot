//#define DEBUG

using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Data.SqlClient;
using Discord;

// REFACTOR
namespace BigMoBot
{
    public static class Globals
    {
        public static readonly ulong BeefBossId = 129692115495157760;
        public static readonly List<ulong> AdminUserIds = new List<ulong> { BeefBossId };

#if (DEBUG)
        public static readonly char CommandPrefix = '?';
#else
        public static readonly char CommandPrefix = '$';
#endif

        public static readonly List<string> LoggingIgnoredCommands = new List<string>
        {
            "greeting"
        };
    }
}
