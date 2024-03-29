﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;

namespace BigMoBot.Modules
{
    public class ChannelDenylist : ModuleBase<SocketCommandContext>
    {
        [Command("denylist")]
        public async Task Task1(string Operation = "", SocketGuildChannel Channel = null)
        {
            var GuildUser = Context.Guild.GetUser(Context.User.Id);
            if (!GuildUser.GuildPermissions.Administrator && !Globals.AdminUserIds.Contains(Context.Message.Author.Id))
                throw new Exception("You do not have permission to run that command");
            else
            {
                var dbContext = await DbHelper.GetDbContext(Context.Guild.Id);
                int CallingUserId = await Globals.GetDbUserId(Context.Guild.Id, Context.Message.Author);
                var AppState = await dbContext.AppStates.AsAsyncEnumerable().FirstOrDefaultAsync();

                if (!AppState.EnableStatisticsTracking)
                    throw new Exception("The [Statistics Tracking] feature is not enabled");

                Operation = Operation.ToLower();
                if (Operation == "add")
                {
                    if (Channel != null)
                    {
                        int dbChannelId = await Globals.GetDbChannelId(Channel);
                        var BlacklistedChannel = await dbContext.ChannelBlacklists.ToAsyncEnumerable().Where(c => c.ChannelId == dbChannelId).FirstOrDefaultAsync();
                        if (BlacklistedChannel != null)
                            await ReplyAsync("Channel <#" + Channel.Id + "> already on the deny list");
                        else
                        {
                            try
                            {
                                Database.ChannelBlacklist NewRow = new Database.ChannelBlacklist();
                                NewRow.ChannelId = dbChannelId;
                                dbContext.ChannelBlacklists.Add(NewRow);
                                await dbContext.SaveChangesAsync();
                                await ReplyAsync("Successfully blocked channel <#" + Channel.Id + "> from statistics");
                                Globals.LogActivity(Context.Guild.Id, 8, "", Channel.Name, true, CallingUserId);
                            }
                            catch (Exception e)
                            {
                                Globals.LogActivity(Context.Guild.Id, 8, Channel.Name, e.Message, false, CallingUserId);
                                throw new Exception("Operation failed: " + e.Message);
                            }
                        }
                    }
                    else
                        throw new Exception("Channel cannot be null");
                }
                else if (Operation == "remove")
                {
                    if (Channel != null)
                    {
                        int dbChannelId = await Globals.GetDbChannelId(Channel);
                        var BlacklistedChannel = await dbContext.ChannelBlacklists.ToAsyncEnumerable().Where(c => c.ChannelId == dbChannelId).FirstOrDefaultAsync();
                        if (BlacklistedChannel == null)
                            await ReplyAsync("Channel <#" + Channel.Id + "> is not blocked");
                        else
                        {
                            try
                            {
                                dbContext.ChannelBlacklists.Remove(BlacklistedChannel);
                                await dbContext.SaveChangesAsync();
                                await ReplyAsync("Successfully removed channel <#" + Channel.Id + "> from the deny list");
                                Globals.LogActivity(Context.Guild.Id, 9, "", Channel.Name, true, CallingUserId);
                            }
                            catch (Exception e)
                            {
                                Globals.LogActivity(Context.Guild.Id, 9, Channel.Name, e.Message, false, CallingUserId);
                                throw new Exception("Operation failed: " + e.Message);
                            }
                        }
                    }
                    else
                        throw new Exception("Channel cannot be null");
                }
                else if (Operation == "list")
                {
                    string ListString = "";
                    var Blacklist = await dbContext.ChannelBlacklists.AsAsyncEnumerable().ToListAsync();
                    var AllChannels = await dbContext.Channels.ToAsyncEnumerable().Where(c => Blacklist.Exists(b => b.ChannelId == c.Id)).ToListAsync();

                    foreach (Database.ChannelBlacklist Item in Blacklist)
                    {
                        var FoundChannel = AllChannels.Find(c => c.Id == Item.ChannelId);
                        if (FoundChannel != null && !FoundChannel.Deleted)
                            ListString += "<#" + FoundChannel.DiscordChannelId.ToInt64() + ">\n";
                    }

                    var embed = new EmbedBuilder
                    {
                        Title = "Channel Denylist",
                        Description = ListString
                    };

                    await ReplyAsync(embed: embed.Build());
                }
                else
                    throw new Exception("Must provide a valid operation (add, remove, list)");

                //await Context.Message.DeleteAsync();
            }
        }
    }
}
