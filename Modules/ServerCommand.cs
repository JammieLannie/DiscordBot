using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot.Modules
{
    public class ServerCommand : InteractiveBase
    {
        [Command("mute")]
        [Description("Takeaway someone muted roles")]
        [Summary("Mute someone. Need admin perm & bot manage role perm")]
        //[RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Mute(IGuildUser user)
        {
            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);

            var builder = new EmbedBuilder()
                .WithTitle("Logged Information")
                .AddField("User", $"{user.Mention}")
                .AddField("Moderator", $"{Context.User.Mention}")
                .AddField("Other Information", "Violate rules / Personal")
                .WithDescription(
                    $"This user has been muted from {Context.Guild.Name} by {Context.User.Username}")
                .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                .WithCurrentTimestamp()
                .WithColor(new Color(54, 57, 62));

            if (!(Context.User is SocketGuildUser userSend)
                || !(userSend.GuildPermissions.KickMembers
                     || user.GuildPermissions.BanMembers
                     || user.GuildPermissions.ManageRoles))
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            bool roleExist = false;

            ulong roleId = 0;

            foreach (var gRole in Context.Guild.Roles)
            {
                if (gRole.Name.ToLower().Equals("muted"))
                {
                    roleExist = true;
                    roleId = gRole.Id;
                }
                else
                {
                    continue;
                }
            }
            if (!roleExist)
            {
                var roleCreation =
                    await Context.Guild.CreateRoleAsync("Muted", GuildPermissions.None, null, false, false);
                try
                {
                    await user.AddRoleAsync(roleCreation);
                    await Context.Channel.SendMessageAsync(null, false, builder.Build());

                    foreach (var channel in Context.Guild.Channels)
                    {
                        await channel.AddPermissionOverwriteAsync(roleCreation,
                            OverwritePermissions.DenyAll(channel).Modify(
                                sendMessages: PermValue.Deny,
                                viewChannel: PermValue.Allow,
                                readMessageHistory: PermValue.Allow)
                        );
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error :" + e);
                }
            }
            else
            {
                var mutedRole = Context.Guild.GetRole(roleId);
                await user.AddRoleAsync(mutedRole);

                await Context.Channel.SendMessageAsync(null, false, builder.Build());
            }
        }

        [Command("unmute")]
        [Description("Takeaway someone muted roles")]
        [Summary("Unmute someone. Need admin perm & bot manage role perm")]
        //[RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task UnMute(IGuildUser user)
        {
            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);

            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower().Equals("muted"));

            if (role == null || !(Context.User is SocketGuildUser userSend) ||
                !(userSend.GuildPermissions.KickMembers ||
                  user.GuildPermissions.BanMembers || user.GuildPermissions.ManageRoles))
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            await user.RemoveRoleAsync(role);

            var builder = new EmbedBuilder()
                .WithTitle("Logged Information")
                .AddField("User", $"{user.Mention}")
                .AddField("Moderator", $"{Context.User.Mention}")
                .AddField("Other Information", "Released from jail!!")
                .WithDescription(
                    $"This user has been unmuted from {Context.Guild.Name} by {Context.User.Username}")
                .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                .WithCurrentTimestamp()
                .WithColor(new Color(54, 57, 62));
            await Context.Channel.SendMessageAsync(null, false, builder.Build());
        }

        [Command("Nuke", RunMode = RunMode.Async)]
        [Description("Clone channel and create new one")]
        [Summary("Nuke a channel")]
        [RequireBotPermission(GuildPermission.Administrator)]
        //[RequireUserPermission(GuildPermission.Administrator)]
        public async Task NukeChannel()
        {
            if (!(Context.User is SocketGuildUser userSend) || !userSend.GuildPermissions.Administrator)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            if (userSend.GuildPermissions.Administrator)
            {
                await ReplyAndDeleteAsync("Are you sure you want to nuke this channel ?",
                    timeout: TimeSpan.FromSeconds(15));
                var response = await NextMessageAsync();
                if (response != null)
                {
                    if (response.ToString().ToLower().Equals("yes"))
                    {
                        var channel = Context.Channel;
                        var oldChannel = ((ITextChannel)channel);
                        var guild = Context.Guild;

                        await ReplyAsync($"Nuking this channel {Context.Channel.Name} in 10s");
                        await Task.Delay(10000);

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
                        return;
                    }
                    else if (response.ToString().ToLower().Equals("no"))
                    {
                        await ReplyAsync($"Nuke cancelled on {Context.Channel.Name}");
                        await Task.Delay(10000);
                        return;
                    }
                }
                else
                {
                    await ReplyAsync($"{Context.User.Mention}, command timed out...");
                }
            }
        }
    }
}