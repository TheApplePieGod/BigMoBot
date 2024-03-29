﻿using System;
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

namespace BigMoBot.Modules
{
    public class Break : ModuleBase<SocketCommandContext>
    {
        [Command("break")]
        public async Task Task2(string MentionedUser = "")
        {
            await ReplyAsync("This command has been replaced with $breakchain to prevent confusion");
        }

        [Command("breakchain")]
        public async Task Task1(string MentionedUser)
        {
            string FormattedId = new string(MentionedUser.Where(char.IsNumber).ToArray());
            var User = await Context.Client.Rest.GetGuildUserAsync(Context.Guild.Id, ulong.Parse(FormattedId));

            if (User == null)
                throw new Exception("User not found");

            var GuildUser = Context.Guild.GetUser(Context.User.Id);
            if (!GuildUser.GuildPermissions.Administrator && !Globals.AdminUserIds.Contains(Context.Message.Author.Id))
                throw new Exception("You do not have permission to run that command");
            else
            {
                var dbContext = await DbHelper.GetDbContext(Context.Guild.Id);
                var AppState = await dbContext.AppStates.AsAsyncEnumerable().FirstOrDefaultAsync();

                if (!AppState.EnableHelloChain)
                    throw new Exception("The [Hello Chain] feature is not enabled");

                var ResponseChannel = Context.Guild.DefaultChannel;
                if (AppState.ResponseChannelId != null && AppState.ResponseChannelId.Length > 0)
                    ResponseChannel = Context.Client.GetChannel(AppState.ResponseChannelId.ToInt64()) as SocketTextChannel;
                int CallingUserId = await Globals.GetDbUserId(Context.Guild.Id, Context.Message.Author);

                try
                {
                    var dbChannel = await dbContext.Channels.ToAsyncEnumerable().Where(c => c.Id == AppState.HelloChannelId).FirstOrDefaultAsync();
                    var HelloChannel = Context.Client.GetChannel(dbChannel.DiscordChannelId.ToInt64()) as SocketTextChannel;
                    await HelloChannel.DeleteAsync();
                    dbChannel.Deleted = true;
                    Globals.LogActivity(Context.Guild.Id, 4, "From command", "Iteration: " + AppState.HelloIteration, true, CallingUserId);
                }
                catch (Exception e)
                {
                    Globals.LogActivity(Context.Guild.Id, 4, "From command", "Iteration: " + AppState.HelloIteration + " Error: " + e.Message, false, CallingUserId);
                    await ReplyAsync("Operation failed: " + e.Message);
                    throw new Exception("Operation failed: " + e.Message);
                }

                int dbUserId = await Globals.GetDbUserId(Context.Guild.Id, User);
                var dbUser = await dbContext.Users.ToAsyncEnumerable().Where(u => u.Id == dbUserId).FirstOrDefaultAsync();
                dbUser.ChainBreaks = dbUser.ChainBreaks + 1;

                try
                {
                    Globals.AwardChainKeeper(ResponseChannel, AppState.HelloIteration, dbUserId, Context.Guild, Context.Client);
                    Globals.SetSuspendedUser(ResponseChannel, dbUserId, Context.Guild, Context.Client);
                }
                catch (Exception e)
                {
                    Globals.LogActivity(Context.Guild.Id, 1, "Failed updating roles after break", "Error: " + e.Message, false, CallingUserId);
                    await ReplyAsync("Failed to update roles");
                }

                AppState.HelloTimerNotified = false;
                if (AppState.AutoCreateNewHello)
                {
                    await ResponseChannel.SendMessageAsync("<@!" + Context.User.Id + "> has decided that <@!" + User.Id + "> has broken the chain. A new chain has been created");

                    try
                    {
                        AppState.HelloIteration = AppState.HelloIteration + 1;
                        var NewChannel = await Context.Guild.CreateTextChannelAsync("hello-chain-" + AppState.HelloIteration, x =>
                        {
                            if (AppState.HelloCategoryId != null && AppState.HelloCategoryId.Length > 0)
                                x.CategoryId = AppState.HelloCategoryId.ToInt64();
                            x.Topic = AppState.HelloTopic;
                        });
                        AppState.HelloChannelId = await Globals.GetDbChannelId(Context.Guild.Id, NewChannel.Id, NewChannel.Name, 2);
                        AppState.HelloDeleted = false;
                        AppState.LastHelloMessage = DateTime.Now;
                        AppState.LastHelloUserId = 0;
                        Globals.LogActivity(Context.Guild.Id, 5, "From command", "Iteration: " + AppState.HelloIteration, true, CallingUserId);
                    }
                    catch (Exception e)
                    {
                        Globals.LogActivity(Context.Guild.Id, 5, "From command", "Iteration: " + AppState.HelloIteration + " Error: " + e.Message, false, CallingUserId);
                        await ReplyAsync("Operation failed: " + e.Message);
                        throw new Exception("Operation failed: " + e.Message);
                    }
                }
                else
                {
                    await ResponseChannel.SendMessageAsync("<@!" + Context.User.Id + "> has decided that <@!" + User.Id + "> has broken the chain. The channel has been deleted");
                    AppState.HelloDeleted = true;
                }

                if (AppState.ChainBreakerRoleId != null && AppState.ChainBreakerRoleId.Length > 0)
                {
                    var BreakerRole = Context.Guild.GetRole(AppState.ChainBreakerRoleId.ToInt64());
                    await (User as IGuildUser).AddRoleAsync(BreakerRole);
                }

                await dbContext.SaveChangesAsync();
                //await Context.Message.DeleteAsync();
            }
        }
    }
}
