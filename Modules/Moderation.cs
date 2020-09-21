using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot.Modules
{
    [Summary(":shield:")]
    public class Moderation : ModuleBase
    {
        [Command("purge")]
        [Summary("Purge message from channel")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int amount)
        {
            if (amount <= 0)
            {
                await Context.Channel.SendMessageAsync("No input");
                return;
            }

            var messages = (await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync()).ToList();
            await ((SocketTextChannel) Context.Channel).DeleteMessagesAsync(messages);

            var message =
                await Context.Channel.SendMessageAsync($"{messages.Count} messages deleted successfully!");

            await Task.Delay(2500);

            await message.DeleteAsync();
        }

        [Command("kick")]
        [Description("Kick someone's ass")]
        [Summary("Kick someone. Need admin perm and bot kick perm")]
        //[RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Kick(IGuildUser userAccount)
        {
            await Context.Channel.DeleteMessageAsync(Context.Message);

            if (!(Context.User is SocketGuildUser user)) return;

            var builder = new EmbedBuilder()
                .WithTitle("Logged Information")
                .AddField("User", $"{userAccount.Mention}")
                .AddField("Moderator", $"{user.Mention}")
                .AddField("Other Information", "Can join server again")
                .WithDescription(
                    $"You can't kick this {user} from {Context.Guild.Name}")
                .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                .WithCurrentTimestamp()
                .WithColor(new Color(54, 57, 62));

            if (user.GuildPermissions.KickMembers)
            {
                await userAccount.KickAsync();
                builder.WithDescription(
                    $"This user has been kicked from {Context.Guild.Name} by {Context.User.Username}");
            }

            await Context.Channel.SendMessageAsync(null, false, builder.Build());
        }

        [Command("ban")]
        [Description("Kick someone's ass")]
        [Summary("Ban someone. Need admin perm and bot ban perm")]
        //[RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(IGuildUser userAccount)
        {
            await Context.Channel.DeleteMessageAsync(Context.Message);

            if (!(Context.User is SocketGuildUser user)) return;

            var builder = new EmbedBuilder()
                .WithTitle("Logged Information")
                .AddField("User", $"{userAccount.Mention}")
                .AddField("Invoked by", $"{user.Mention}")
                .WithDescription(
                    $"You can't ban this {user} from {Context.Guild.Name}")
                .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                .WithCurrentTimestamp()
                .WithColor(new Color(54, 57, 62));

            if (user.GuildPermissions.BanMembers)
            {
                await userAccount.BanAsync();
                builder.WithDescription(
                    $"This user has been banned from {Context.Guild.Name} by {Context.User.Username}");
            }

            await Context.Channel.SendMessageAsync(null, false, builder.Build());
        }

        [Command("verify", true)]
        [Summary("Verify user. Server need to have Verified role")]
        public async Task RoleTask()
        {
            var user = Context.User as SocketGuildUser;

            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);

            var role = ((IGuildUser) user)?.Guild.Roles.FirstOrDefault(x => x.Name.Equals("Verified"));

            var verifyUser = Context.User.Mention;

            if (role == null) return;
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

        [Command("Create", true)]
        [Summary("Create a new role")]
        [RequireBotPermission(GuildPermission.Administrator)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task CreateRole([Remainder] string role)
        {
            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);

            if (((SocketCommandContext) Context).IsPrivate || role == null) return;
            await Context.Guild.CreateRoleAsync(role, GuildPermissions.None, null, false, false);
        }

        [Command("revoke", true)]
        [Description("Revoke a role from someone")]
        [Summary("Revoke someone role. Need admin perm & bot manage role perm")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        //[RequireBotPermission(GuildPermission.Administrator)]
        public async Task RevokeRole(SocketGuildUser user, [Remainder] string msg)
        {
            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);

            if (((SocketCommandContext) Context).IsPrivate || msg == null) return;
            msg = msg.ToLower();

            var role = user.Guild.Roles.FirstOrDefault(x =>
                x.Name.ToLower().Equals(msg) || x.Id.ToString().Equals(msg) || x.Name.ToLower().Contains(msg));

            if (role == null || !(Context.User is SocketGuildUser userSend) || !userSend.GuildPermissions.ManageRoles ||
                !Utils.CanInteractRole(userSend, role))
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            var builder = new EmbedBuilder()
                .WithTitle("Logged Information")
                .AddField("User", $"{user.Mention}")
                .AddField("Moderator", $"{Context.User.Mention}")
                .WithDescription(
                    $"{role} does not exist from {user}")
                .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                .WithCurrentTimestamp()
                .WithColor(new Color(54, 57, 62));
            if (user.Roles.Contains(role))
            {
                await user.RemoveRoleAsync(role);
                builder.WithDescription($"{role} has been revoke from {user} by {Context.User.Username}");
            }

            await Context.Channel.SendMessageAsync(null, false, builder.Build());
        }

        [Command("give", true)]
        [Description("Give a role to someone")]
        [Summary("Grant someone role. Need admin perm & bot manage role perm")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        //[RequireBotPermission(GuildPermission.Administrator)]
        public async Task AddRole(SocketGuildUser user, [Remainder] string msg)
        {
            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);
            if (((SocketCommandContext) Context).IsPrivate || msg == null) return;
            msg = msg.ToLower();

            var role = user.Guild.Roles.FirstOrDefault(x =>
                x.Name.ToLower().Equals(msg) || x.Id.ToString().Equals(msg) || x.Name.ToLower().Contains(msg));

            if (role == null || !(Context.User is SocketGuildUser userSend) || !userSend.GuildPermissions.ManageRoles ||
                !Utils.CanInteractRole(userSend, role))
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            var builder = new EmbedBuilder()
                .WithTitle("Logged Information")
                .AddField("User", $"{user.Mention}")
                .AddField("Moderator", $"{Context.User.Mention}")
                .WithDescription(
                    $"{user} already has the role {role}")
                .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                .WithCurrentTimestamp()
                .WithColor(new Color(54, 57, 62));

            if (!user.Roles.Contains(role))
            {
                await user.AddRoleAsync(role);
                builder.WithDescription($"{user} has been granted {role} by {Context.User.Username}");
            }

            await Context.Channel.SendMessageAsync(null, false, builder.Build());
        }

    }
}