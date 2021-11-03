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
        private static List<string> Greetings = new List<string>();
        private static List<string> BotBoss = new List<string>();

        public Fun()
        {
            var assembly = Assembly.GetExecutingAssembly();
            if (Greetings.Count == 0)
            {
                var GreetingsFileName = "BigMoBot.Data.Words.txt";
                using (Stream stream = assembly.GetManifestResourceStream(GreetingsFileName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string l = "";
                    while ((l = reader.ReadLine()) != null) { Greetings.Add(l); }
                }
            }
            if (BotBoss.Count == 0)
            {
                string[] BotBossFileNames = { "BigMoBot.Data.BotBoss1.txt", "BigMoBot.Data.BotBoss2.txt" };
                for (int i = 0; i < BotBossFileNames.Length; i++)
                {
                    using (Stream stream = assembly.GetManifestResourceStream(BotBossFileNames[i]))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string l = "";
                        string buffer = "";
                        while ((l = reader.ReadLine()) != null)
                        {
                            if (l.StartsWith("===========")) // end of section
                            {
                                BotBoss.Add(buffer);
                                buffer = "";
                            }
                            else
                                buffer += l + '\n';
                        }
                        if (buffer != "")
                            BotBoss.Add(buffer);
                    }
                }
            }
        }

        [Command("greeting")]
        public async Task Task1(int Amount = 1)
        {
            var dbContext = await DbHelper.GetDbContext(Context.Guild.Id);
            var AppState = await dbContext.AppStates.AsAsyncEnumerable().FirstOrDefaultAsync();

            if (!AppState.EnableHelloChain)
                throw new Exception("The [Hello Chain] feature is not enabled");

            if (Amount == 0)
                throw new Exception("Amount cannot be zero");
            if (Amount > 10)
                throw new Exception("Amount cannot be more than 10");

            Random ran = new Random();
            string Reply = "";

            for (int i = 0; i < Amount; i++)
            {
                int Index = ran.Next(0, Greetings.Count);
                Reply += "- " + Greetings[Index] + '\n';
            }

            await ReplyAsync(Reply);
        }

        [Command("botboss")]
        public async Task Task2(int SampleId = -1)
        {
            Random ran = new Random();

            int Index = SampleId;
            if (Index == -1)
                Index = ran.Next(0, BotBoss.Count);
            else if (Index >= BotBoss.Count)
                throw new Exception("Invalid sample ID");

            string Reply = BotBoss[Index];

            var embed = new EmbedBuilder
            {
                Title = "BotBoss v1",
                Description = "Sample #" + Index + "/" + (BotBoss.Count - 1)
            };
            embed.AddField("Text", Reply);

            await ReplyAsync(embed: embed.Build());
        }
    }
}
