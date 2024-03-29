﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace BigMoBot.Modules
{
    public class MessageUser : ModuleBase<SocketCommandContext>
    {
        [Command("messageuser")]
        public async Task Task1(ulong ReplyingUser, string Message)
        {
            if (Globals.AdminUserIds.Contains(Context.Message.Author.Id)) // admin command
            {
                var User = await Context.Client.Rest.GetUserAsync(ReplyingUser);
                await User.SendMessageAsync(Message);
                await ReplyAsync("Sent");
            }
        }
    }
}
