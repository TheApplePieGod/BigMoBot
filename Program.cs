using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;
using System.IO;
using DbUp;
using Microsoft.Extensions.Configuration;

namespace BigMoBot
{
    public class Program
    {
        static void Main(string[] args)
        => new Program().StartAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        private CommandHandler _handler;

        public Task OnLogAsync(LogMessage msg)
        {
            Console.WriteLine(msg);
            return Task.CompletedTask;
        }

        public async Task StartAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers
            });
            _client.Log += OnLogAsync;

            var Credential = "";
#if (DEBUG)
            var CredentialFile = "Debug/BotCredential.txt";
            using (StreamReader reader = new StreamReader(CredentialFile))
                Credential = reader.ReadToEnd().Trim();
#else
            Credential = Environment.GetEnvironmentVariable("DISCORD_CREDENTIAL");
#endif

            await _client.LoginAsync(TokenType.Bot, Credential);
            await _client.StartAsync();

            _handler = new CommandHandler(_client);

            await Task.Delay(-1);
        }
        
    }
}