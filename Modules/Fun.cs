using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using BigMoBot.Database.FunctionModels;
using System.IO;
using System.Reflection;

namespace BigMoBot.Modules
{
    public class Fun : ModuleBase<SocketCommandContext>
    {
        private List<string> Greetings = new List<string>();

        public Fun()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var GreetingsFileName = "BigMoBot.Data.Words.txt";
            using (Stream stream = assembly.GetManifestResourceStream(GreetingsFileName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string l = "";
                while ((l = reader.ReadLine()) != null) { Greetings.Add(l); }
            }
        }

        [Command("greeting")]
        public async Task Task1()
        {
            Random ran = new Random();
            int Index = ran.Next(0, Greetings.Count);
            await ReplyAsync(Greetings[Index]);
        }
    }
}
