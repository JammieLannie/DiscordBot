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
    [Summary(":shield:")]
    public class Moderation : InteractiveBase
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
                || !(userSend.GuildPermissions.ManageRoles
                     || userSend.GuildPermissions.KickMembers
                     || user.GuildPermissions.BanMembers
                     || user.GuildPermissions.ManageRoles
                     || Utils.CanInteractUser(userSend, (SocketGuildUser)user)))
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            var mutedRole = Context.Guild.Roles.FirstOrDefault(t => t.Name.ToLower().Equals("muted"));
            if (mutedRole == null)
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
                    Console.WriteLine(e);
                }
            }
            else
            {
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

        [Command("kick")]
        [Description("Kick someone's ass")]
        [Summary("Kick someone. Need admin perm and bot kick perm")]
        //[RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Kick(SocketGuildUser userAccount, [Remainder] string reason = null)
        {
            await Context.Channel.DeleteMessageAsync(Context.Message);

            if (!(Context.User is SocketGuildUser userSend) || !userSend.GuildPermissions.ManageRoles ||
                !Utils.CanInteractUser(userSend, userAccount))
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }
            var builder = new EmbedBuilder();

            builder.WithTitle("Logged Information")
                .AddField("User", $"{userAccount.Mention}")
                .AddField("Command issued by", $"{userSend.Mention}")
                .WithDescription(
                    $"You can't kick this {userAccount} from {Context.Guild.Name}")
                .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                .WithCurrentTimestamp()
                .WithColor(new Color(54, 57, 62));

            if (userSend.GuildPermissions.KickMembers)
            {
                await userAccount.KickAsync(reason);
                builder.AddField("Reason", $"{reason}")
                    .AddField("Other Information", "Can join server again")
                    .WithDescription(
                        $"This user has been kicked from {Context.Guild.Name} by {Context.User.Username}!");
            }

            await Context.Channel.SendMessageAsync(null, false, builder.Build());
        }

        [Command("ban")]
        [Description("Ban someone's ass")]
        [Summary("Ban someone. Need admin perm and bot ban perm")]
        //[RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(SocketGuildUser userAccount, [Remainder] string reason = null)
        {
            await Context.Channel.DeleteMessageAsync(Context.Message);

            if (!(Context.User is SocketGuildUser userSend) || !userSend.GuildPermissions.ManageRoles ||
                !Utils.CanInteractUser(userSend, userAccount))
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }
            var builder = new EmbedBuilder();

            builder.WithTitle("Logged Information")
                .AddField("User", $"{userAccount.Mention}")
                .AddField("Command issued by", $"{userSend.Mention}")
                .WithDescription(
                    $"You can't ban this {userAccount} from {Context.Guild.Name}")
                .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                .WithCurrentTimestamp()
                .WithColor(new Color(54, 57, 62));

            if (userSend.GuildPermissions.BanMembers)
            {
                await userAccount.BanAsync(0, reason);
                builder.AddField("Reason", $"{reason}")
                    .AddField("Other Information", "Can't join server again")
                    .WithDescription(
                        $"This user has been banned from {Context.Guild.Name} by {Context.User.Username}!");
            }

            await Context.Channel.SendMessageAsync(null, false, builder.Build());
        }

        [Command("banserver")]
        [Description("Ban user has mutual guilds with bot")]
        [Summary("Ban someone share mutual guilds with bot. Need admin perm and bot ban perm")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanAll(SocketGuildUser user, [Remainder] string reason = null)
        {
            if (!(Context.User is SocketGuildUser userSend) || !userSend.GuildPermissions.ManageRoles ||
                !Utils.CanInteractUser(userSend, user))
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }
            foreach (var guilds in user.MutualGuilds)
                await guilds.AddBanAsync(user, 1, $"Banned by {Context.User}. Reason: {reason}");

            await ReplyAsync($"{user} was successfully Banned from all servers");
        }

        [Command("verify", true)]
        [Summary("Verify user. Server need to have Verified role")]
        public async Task RoleTask()
        {
            var user = Context.User as SocketGuildUser;

            if (user == null) return;

            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);

            var role = ((IGuildUser) user)?.Guild.Roles.FirstOrDefault(x => x.Name.Equals("Verified"));

            var verifyUser = Context.User.Mention;

            role ??= await Context.Guild.CreateRoleAsync("Verified", Utils.MemPermissions, null, false, false);

            if (user.Roles.Contains(role))
            {
                var message = await Context.Channel.SendMessageAsync($"{verifyUser} has already verified");

                await Task.Delay(5000);

                await message.DeleteAsync();
            }
            else
            {
                await ((SocketGuildUser) Context.User).AddRoleAsync(role);

                var message =
                    await Context.Channel.SendMessageAsync(
                        $"{verifyUser} has been verified\n You can now chat with others");

                await Task.Delay(5000);

                await message.DeleteAsync();
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
    }
}