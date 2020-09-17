using System;
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
        public static bool CanInteractRole(SocketGuildUser user, SocketRole role) => user.Roles.Select(r => r.Position).Prepend(0).Max() > role.Position;

        [Command("purge")]
        [Summary("Purge message from channel")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int amount)
        {
            if (amount > 0)
            {
                var messages = (await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync()).ToList();
                await ((SocketTextChannel)Context.Channel).DeleteMessagesAsync(messages);

                var message = await Context.Channel.SendMessageAsync($"{messages.Count} messages deleted successfully!");

                await Task.Delay(2500);

                await message.DeleteAsync();
            }
            else if (amount == 0)
            {
                await Context.Channel.SendMessageAsync("No input");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Error occurred");
            }
        }

        [Command("kick")]
        [Description("Kick someone's ass")]
        [Summary("Kick someone. Need admin perm and bot kick perm")]
        //[RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Kick(IGuildUser userAccount)
        {
            var message = Context.Message;

            await Context.Channel.DeleteMessageAsync(message);

            if (!(Context.User is SocketGuildUser user)) return;

            if (user.GuildPermissions.KickMembers)
            {
                await userAccount.KickAsync();

                var builder = new EmbedBuilder()
                    .WithTitle("Logged Information")
                    .AddField("User", $"{userAccount.Mention}")
                    .AddField("Moderator", $"{user.Mention}")
                    .AddField("Other Information", "Can join server again")
                    .WithDescription(
                        $"This user has been kicked from {Context.Guild.Name} by {Context.User.Username}")
                    .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                    .WithCurrentTimestamp()
                    .WithColor(new Color(54, 57, 62));
                var embed = builder.Build();

                await Context.Channel.SendMessageAsync(null, false, embed);
            }
            else
            {
                var builder = new EmbedBuilder()
                    .WithTitle("Logged Information")
                    .AddField("User", $"{userAccount.Mention}")
                    .AddField("Invoked by", $"{user.Mention}")
                    .WithDescription(
                        $"You can't kick this {user} from {Context.Guild.Name}")
                    .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                    .WithCurrentTimestamp()
                    .WithColor(new Color(54, 57, 62));
                var embed = builder.Build();

                await Context.Channel.SendMessageAsync(null, false, embed);

                //await Context.Channel.SendMessageAsync($"{user} doesn't have permissions to kick this user.");
            }
        }

        [Command("ban")]
        [Description("Kick someone's ass")]
        [Summary("Ban someone. Need admin perm and bot ban perm")]
        //[RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(IGuildUser userAccount)
        {
            var message = Context.Message;

            await Context.Channel.DeleteMessageAsync(message);

            if (!(Context.User is SocketGuildUser user)) return;

            if (user.GuildPermissions.BanMembers)
            {
                await userAccount.BanAsync();

                var builder = new EmbedBuilder()
                    .WithTitle("Logged Information")
                    .AddField("User", $"{userAccount.Mention}")
                    .AddField("Moderator", $"{user.Mention}")
                    .AddField("Other Information", "Can't join server again")
                    .WithDescription(
                        $"This user has been banned from {Context.Guild.Name} by {Context.User.Username}")
                    .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                    .WithCurrentTimestamp()
                    .WithColor(new Color(54, 57, 62));
                var embed = builder.Build();

                await Context.Channel.SendMessageAsync(null, false, embed);
            }
            else
            {
                //await Context.Channel.SendMessageAsync($"{user} doesn't have permissions to ban this user.");
                var builder = new EmbedBuilder()
                    .WithTitle("Logged Information")
                    .AddField("User", $"{userAccount.Mention}")
                    .AddField("Invoked by", $"{user.Mention}")
                    .WithDescription(
                        $"You can't ban this {user} from {Context.Guild.Name}")
                    .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                    .WithCurrentTimestamp()
                    .WithColor(new Color(54, 57, 62));
                var embed = builder.Build();

                await Context.Channel.SendMessageAsync(null, false, embed);
            }
        }

        [Command("verify", true)]
        [Summary("Verify user. Server need to have Verified role")]
        public async Task RoleTask()
        {
            var user = Context.User as SocketGuildUser;

            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);

            var roleId = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name.Equals("Verified"));

            var verifyUser = Context.User.Mention;

            if (roleId != null)
            {
                if (user.Roles.Contains(roleId))
                {
                    var message = await Context.Channel.SendMessageAsync($"{verifyUser} has already verified");

                    await Task.Delay(5000);

                    await message.DeleteAsync();

                }
                else
                {
                    await ((SocketGuildUser)Context.User).AddRoleAsync(roleId);

                    var message = await Context.Channel.SendMessageAsync($"{verifyUser} has been verified\n You can now chat with others");

                    await Task.Delay(5000);

                    await message.DeleteAsync();

                }
            }
            else
            {
                var message =
                    await Context.Channel.SendMessageAsync($"Role Verified not existed yet on {Context.Guild.Name}!");
            }
        }

        /*
        [Command("Nuke")
        [Description("Clone channel and delete old one")]
        [Summary("Nuke a channel")]
        [RequireBotPermission(GuildPermission.Administrator)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Nuke()
        {

        }

        */

        /*
        [Command("Create", true)]
        [Description("Create a new role with basic perm")]
        [Summary("Create a new role")]
        [RequireBotPermission(GuildPermission.Administrator)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task CreateRole()
        {
            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);

            var mentionUser = Context.User.Mention;

            var msg = Context.Message.Content;

            if (((SocketCommandContext) Context).IsPrivate || msg == null) return;
            msg = msg.ToLower();

            var role = await Context.Guild.CreateRoleAsync($"{msg}");

            var serverRole = Context.Guild.Roles.ToString();

            if (!(Context.User is SocketGuildUser user)) return;

            if (user.GuildPermissions.ManageRoles)
            {
                await Context.Guild.CreateRoleAsync();
            }
            else
            {

            }
        }
        */

        [Command("revoke", true)]
        [Description("Revoke a role from someone")]
        [Summary("Revoke someone role. Need admin perm & bot manage role perm")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        //[RequireBotPermission(GuildPermission.Administrator)]
        public async Task RevokeRole(SocketGuildUser user, [Remainder] string msg)
        {
            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);

            var mentionUser = user.Mention;
            
            if (((SocketCommandContext)Context).IsPrivate || msg == null) return;
            msg = msg.ToLower();
            
            var role = user.Guild.Roles.FirstOrDefault(x => 
                x.Name.ToLower().Equals(msg) || x.Id.ToString().Equals(msg) || x.Name.ToLower().Contains(msg));

            if (role == null) return;

            if (!(Context.User is SocketGuildUser userSend)) return;

            if (userSend.GuildPermissions.ManageRoles)
            {

                if (!user.Roles.Contains(role))
                {

                    var builder = new EmbedBuilder()
                        .WithTitle("Logged Information")
                        .AddField("User", $"{user.Mention}")
                        .AddField("Moderator", $"{Context.User.Mention}")
                        .WithDescription(
                            $"{role} does not exist from {user}")
                        .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                        .WithCurrentTimestamp()
                        .WithColor(new Color(54, 57, 62));
                    var embed = builder.Build();

                    await Context.Channel.SendMessageAsync(null, false, embed);

                    //var message = await Context.Channel.SendMessageAsync($"{role} does not exist from {user.Mention} !");
                    //await Task.Delay(5000);
                    //await message.DeleteAsync();
                }
                else
                {
                    await user.RemoveRoleAsync(role);

                    var builder = new EmbedBuilder()
                        .WithTitle("Logged Information")
                        .AddField("User", $"{user.Mention}")
                        .AddField("Moderator", $"{Context.User.Mention}")
                        .AddField("Role revoked", $"{role.Mention}")
                        .WithDescription(
                            $"{role} has been revoke from {user} by {Context.User.Username}")
                        .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                        .WithCurrentTimestamp()
                        .WithColor(new Color(54, 57, 62));
                    var embed = builder.Build();

                    await Context.Channel.SendMessageAsync(null, false, embed);

                    //var message = await Context.Channel.SendMessageAsync($"{role} has been revoked from {user.Mention} !");
                    //await Task.Delay(5000); 
                    //await message.DeleteAsync();
                }
            }
            else
            {
                var builder = new EmbedBuilder()
                    .WithTitle("Logged Information")
                    .AddField("Command issuer", $"{userSend.Mention}")
                    .WithDescription(
                        $"{Context.User.Mention} does not have the permission to do that.")
                    .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                    .WithCurrentTimestamp()
                    .WithColor(new Color(54, 57, 62));
                var embed = builder.Build();

                await Context.Channel.SendMessageAsync(null, false, embed);
            }

        }

        [Command("give", true)]
        [Description("Give a role to someone")]
        [Summary("Grant someone role. Need admin perm & bot manage role perm")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        //[RequireBotPermission(GuildPermission.Administrator)]
        public async Task AddRole(SocketGuildUser user, [Remainder] string msg)
            {
                await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);

                var mentionUser = user.Mention;

                if (((SocketCommandContext)Context).IsPrivate || msg == null) return;
                    msg = msg.ToLower();
                    
                var role = user.Guild.Roles.FirstOrDefault(x =>
                    x.Name.ToLower().Equals(msg) || x.Id.ToString().Equals(msg) || x.Name.ToLower().Contains(msg));

            if (role == null) return;

            if (!(Context.User is SocketGuildUser userSend)) return;

            if (userSend.GuildPermissions.ManageRoles)
            {

                if (user.Roles.Contains(role))
                {

                    var builder = new EmbedBuilder()
                        .WithTitle("Logged Information")
                        .AddField("User", $"{user.Mention}")
                        .AddField("Moderator", $"{Context.User.Mention}")
                        .WithDescription(
                            $"{user} already has the role {role}")
                        .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                        .WithCurrentTimestamp()
                        .WithColor(new Color(54, 57, 62));
                    var embed = builder.Build();

                    await Context.Channel.SendMessageAsync(null, false, embed);

                    //  var message = await Context.Channel.SendMessageAsync($"{user.Mention} already had the role {role} !");
                    //  await Task.Delay(5000);
                    //  await message.DeleteAsync();
                }
                else
                {
                    await user.AddRoleAsync(role);

                    var builder = new EmbedBuilder()
                        .WithTitle("Logged Information")
                        .AddField("User", $"{user.Mention}")
                        .AddField("Moderator", $"{Context.User.Mention}")
                        .AddField("Role granted", $"{role.Mention}")
                        .WithDescription(
                            $"{user} has been granted {role} by {Context.User.Username}")
                        .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                        .WithCurrentTimestamp()
                        .WithColor(new Color(54, 57, 62));
                    var embed = builder.Build();

                    await Context.Channel.SendMessageAsync(null, false, embed);
                    await user.AddRoleAsync(role);
                    //  var message = await Context.Channel.SendMessageAsync($"{mentionUser} has been granted the role : {role} !");
                    //  await Task.Delay(5000);
                    //  await message.DeleteAsync();
                }
            }
            else
            {
                var builder = new EmbedBuilder()
                    .WithTitle("Logged Information")
                    .AddField("Command issuer", $"{userSend.Mention}")
                    .WithDescription(
                        $"{Context.User.Mention} does not have the permission to do that.")
                    .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                    .WithCurrentTimestamp()
                    .WithColor(new Color(54, 57, 62));
                var embed = builder.Build();

                await Context.Channel.SendMessageAsync(null, false, embed);
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

            var mentionUser = user.Mention;

            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower().Equals("muted"));

            if (role == null) return;

            if (!(Context.User is SocketGuildUser userSend)) return;

            if (userSend.GuildPermissions.ManageRoles)
            {
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
                var embed = builder.Build();

                await Context.Channel.SendMessageAsync(null, false, embed);
            }
            else
            {
                var builder = new EmbedBuilder()
                    .WithTitle("Logged Information")
                    .AddField("Command issuer", $"{userSend.Mention}")
                    .WithDescription(
                        $"{Context.User.Mention} does not have the permission to do that.")
                    .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                    .WithCurrentTimestamp()
                    .WithColor(new Color(54, 57, 62));
                var embed = builder.Build();

                await Context.Channel.SendMessageAsync(null, false, embed);
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

            var mentionUser = user.Mention;

            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower().Equals("muted"));

            if (role == null) return;

            if (!(Context.User is SocketGuildUser userSend)) return;

            if (userSend.GuildPermissions.ManageRoles)
            {

                await user.AddRoleAsync(role);

                //await Context.Channel.SendMessageAsync($"{mentionUser} has been muted from {Context.Guild.Name}");

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
                var embed = builder.Build();

                await Context.Channel.SendMessageAsync(null, false, embed);
            }
            else
            {
                var builder = new EmbedBuilder()
                    .WithTitle("Logged Information")
                    .AddField("Command issuer", $"{userSend.Mention}")
                    .WithDescription(
                        $"{Context.User.Mention} does not have the permission to do that.")
                    .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                    .WithCurrentTimestamp()
                    .WithColor(new Color(54, 57, 62));
                var embed = builder.Build();

                await Context.Channel.SendMessageAsync(null, false, embed);
            }
        }
    }
}