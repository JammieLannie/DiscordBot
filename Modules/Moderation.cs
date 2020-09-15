using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot.Modules
{
    public class Moderation : ModuleBase
    {
        [Command("purge")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int amount)
        {

            if (amount > 0)
            {
                var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

                var message =
                    await Context.Channel.SendMessageAsync($"{messages.Count()} messages deleted successfully!");

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
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Kick(IGuildUser userAccount)
        {
            var message = Context.Message;

            await Context.Channel.DeleteMessageAsync(message);

            var user = Context.User as SocketGuildUser;

            if (user.GuildPermissions.KickMembers)
            {
                await userAccount.KickAsync();
                //await Context.Channel.SendMessageAsync($"The user `{userAccount}` has been kicked from " + guild +
                //                                       $", because {reason}!!");
                var builder = new EmbedBuilder()
                    .WithTitle("Logged Information")
                    .AddField("User", $"{user.Mention}")
                    .AddField("Moderator", $"{Context.User.Username}")
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
                await Context.Channel.SendMessageAsync("No permissions to kick this user.");
            }
           
        }
    
        [Command("ban")]
        [Description("Kick someone's ass")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(IGuildUser userAccount)
        {
            var message = Context.Message;

            await Context.Channel.DeleteMessageAsync(message).ConfigureAwait(false);

            var user = Context.User as SocketGuildUser;

            if (user.GuildPermissions.BanMembers)
            {
                await userAccount.BanAsync();
                //await Context.Channel.SendMessageAsync($"The user `{userAccount}` has been kicked from " + guild +
                //                                       $", because {reason}!!");
                var builder = new EmbedBuilder()
                    .WithTitle("Logged Information")
                    .AddField("User", $"{user.Mention}")
                    .AddField("Moderator", $"{Context.User.Username}")
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
                await Context.Channel.SendMessageAsync("No permissions to kick this user.");
            }

        }

        [Command("verify")]
        public async Task RoleTask()
        {

            var message = Context.Message;

            await Context.Channel.DeleteMessageAsync(message).ConfigureAwait(false);

            var roleId = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToString() == ("Verified"));

            await ((SocketGuildUser)Context.User).AddRoleAsync(roleId);

            var user = Context.User.Mention;

            await Context.Channel.SendMessageAsync($"{user} has been verified\n You can now chat with others");

        }
       
        [Command("unmute")]
        [Description("Takeaway someone muted roles")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task UnMute(IGuildUser user)
        {
            var message = Context.Message;

            await Context.Channel.DeleteMessageAsync(message).ConfigureAwait(false);

            /*
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithColor(Color.DarkTeal)
                .WithCurrentTimestamp()
                .WithAuthor(Context.User)
                .AddField($"{user} has been unmuted", null);
            var embed = builder.Build();

            await Context.Channel.SendMessageAsync(null, false, embed);
            */

            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToString() == ("Muted"));

            await user.RemoveRoleAsync(role);

        }

        [Command("mute")]
        [Description("Takeaway someone muted roles")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Mute(IGuildUser user)
        {
            var message = Context.Message;

            await Context.Channel.DeleteMessageAsync(message).ConfigureAwait(false);
            
            /*
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithColor(Color.DarkTeal)
                .WithCurrentTimestamp()
                .WithAuthor(Context.User)
                .AddField($"{user} has been muted", null);
            var embed = builder.Build();

            await Context.Channel.SendMessageAsync(null, false, embed);
            */

            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToString() == ("Muted"));

            await user.AddRoleAsync(role);

        }

    }
}
