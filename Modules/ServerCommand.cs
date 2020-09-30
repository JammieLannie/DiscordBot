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
    [Summary(":b:")]
    public class ServerCommand : InteractiveBase
    {
        [Command("set")]
        [Description("Change someone nickname")]
        [Summary("Change user nickname")]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        public async Task SetName(IGuildUser user, [Remainder] string nickName)
        {
            if (nickName == null) return;
            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);

            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageNicknames)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            await user.ModifyAsync(c => c.Nickname = nickName);
            var builder = new EmbedBuilder()
                .WithTitle("Name changed")
                .WithDescription(
                    $"{user}'s name has been changed to {nickName}!")
                .WithCurrentTimestamp()
                .WithColor(new Color(54, 57, 62));
            await Context.Channel.SendMessageAsync(null, false, builder.Build());
        }

        [Command("nomention")]
        [Summary("Disable mention everyone / here on roles")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task DisableMention()
        {
            if (Context.User is IGuildUser)
            {
                foreach (var roles in Context.Guild.Roles)
                {
                    try
                    {
                        if (roles.ToString().ToLower().Equals("muted"))
                        {
                            Console.WriteLine($"skipped {roles}");
                            continue;
                        }
                        Console.WriteLine($"Role : {roles}");
                        await roles.ModifyAsync(r => r.Permissions = Utils.MemPermissions);
                    }
                    catch
                    {
                        Console.WriteLine($"{roles} is the same or higher than bot!!");
                    }
                }
                await Context.Channel.SendMessageAsync("Mention disabled on all non-powered roles");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Permission denied!");
            }
        }

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

            ulong roleId = 0;

            foreach (var gRole in Context.Guild.Roles)
                if (gRole.Name.ToLower().Equals("muted"))
                    roleId = gRole.Id;
            if (roleId != 0)
            {
                var roleCreation =
                    await Context.Guild.CreateRoleAsync("Muted", Utils.MutedPermissions, null, false, false);
                try
                {
                    await user.AddRoleAsync(roleCreation);
                    await Context.Channel.SendMessageAsync(null, false, builder.Build());
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
        public async Task UnMute(SocketGuildUser user)
        {
            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);

            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower().Equals("muted"));

            if (role == null || !(Context.User is SocketGuildUser userSend)
                  || !(userSend.GuildPermissions.KickMembers
                  || user.GuildPermissions.BanMembers
                  || user.GuildPermissions.ManageRoles))
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
                        var oldChannel = (ITextChannel)channel;
                        var guild = Context.Guild;

                        await ReplyAsync($"Nuking this channel {Context.Channel.Name} in 6.9s");
                        await Task.Delay(6900);

                        await guild.CreateTextChannelAsync(oldChannel.Name, newChannel =>
                        {
                            newChannel.CategoryId = oldChannel.CategoryId;
                            newChannel.Topic = oldChannel.Topic;
                            newChannel.Position = oldChannel.Position;
                            newChannel.SlowModeInterval = oldChannel.SlowModeInterval;
                            newChannel.IsNsfw = oldChannel.IsNsfw;
                        });
                        await oldChannel.DeleteAsync();
                    }
                    else if (response.ToString().ToLower().Equals("no"))
                    {
                        await ReplyAsync($"Nuke cancelled on {Context.Channel.Name}");
                    }
                }
                else
                {
                    await ReplyAsync($"{Context.User.Mention}, command timed out...");
                }
            }
        }

        [Command("verifyall")]
        [Summary("Grant verified role to all user")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task VerifyAll()
        {
            //check permission from user who issued the command
            if (!(Context.User is SocketGuildUser userSend)
                || !(userSend.GuildPermissions.ManageRoles
                     || userSend.GuildPermissions.Administrator))
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }
            await Context.Channel.SendMessageAsync($"Granting role for {Context.Guild.MemberCount} user");
            var roleVerify = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower().Equals("verified"));
            if (roleVerify == null)
            {
                if (Context.IsPrivate) return;
                if (!(Context.User is SocketGuildUser)) return;
                var role = await Context.Guild.CreateRoleAsync("Verified", Utils.MemPermissions, null, false, false);
                foreach (var user in Context.Guild.Users) await user.AddRoleAsync(role);
                await Context.Channel.SendMessageAsync($"Everyone is now have the role {role}!");
                return;
            }
            if (Context.User is SocketGuildUser)
            {
                foreach (var user in Context.Guild.Users) await user.AddRoleAsync(roleVerify);
                await Context.Channel.SendMessageAsync($"Everyone is now have the role {roleVerify}!");
            }
        }
    }
}