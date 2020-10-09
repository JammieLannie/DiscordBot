using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualBasic;

namespace DiscordBot.Modules
{
    [Summary(":b:")]
    public class ServerCommand : InteractiveBase
    {
        [Command("Create")]
        [Summary("Create a new role")]
        [RequireBotPermission(GuildPermission.Administrator)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task CreateRole([Remainder] string role)
        {
            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);
            if (!(Context.User is SocketGuildUser userSend) || !userSend.GuildPermissions.ManageRoles)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }
            if (Context.IsPrivate || role == null) return;
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

            if (Context.IsPrivate || msg == null) return;
            msg = msg.ToLower();

            var role = user.Guild.Roles.FirstOrDefault(x =>
                x.Name.ToLower().Equals(msg) || x.Id.ToString().Equals(msg) || x.Name.ToLower().Contains(msg));

            var builder = new EmbedBuilder();

            builder.WithTitle("Logged Information")
                .AddField("User", $"{user.Mention}")
                .AddField("Moderator", $"{Context.User.Mention}")
                .WithDescription(
                    $"{role} does not exist from {user}")
                .WithFooter($"{Context.User.Username}", Context.User.GetAvatarUrl())
                .WithCurrentTimestamp()
                .WithColor(new Color(54, 57, 62));

            if (role == null) builder.WithDescription($"This role does not exist in {Context.Guild.Name}!");

            if (!(Context.User is SocketGuildUser userSend) || !userSend.GuildPermissions.ManageRoles ||
                !Utils.CanInteractRole(userSend, role))
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            if (user.Roles.Contains(role))
            {
                await user.RemoveRoleAsync(role);
                builder.WithDescription($"{role} has been revoke from {user} by {Context.User.Username}");
            }

            await Context.Channel.SendMessageAsync(null, false, builder.Build());
        }

        [Command("give")]
        [Description("Give a role to someone")]
        [Summary("Grant someone role. Need admin perm & bot manage role perm")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        //[RequireBotPermission(GuildPermission.Administrator)]
        public async Task AddRole(SocketGuildUser user, [Remainder] string msg)
        {
            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);
            if (Context.IsPrivate || msg == null) return;
            msg = msg.ToLower();

            var role = user.Guild.Roles.FirstOrDefault(x =>
                x.Name.ToLower().Equals(msg) || x.Id.ToString().Equals(msg) || x.Name.ToLower().Contains(msg));

            if (role == null || !(Context.User is SocketGuildUser userSend) || !userSend.GuildPermissions.ManageRoles ||
                !Utils.CanInteractRole(userSend, role))
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }
            var builder = new EmbedBuilder();

            builder.WithTitle("Logged Information")
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

        [Command("lock",true)]
        [Summary("lock channel")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task LockChannel()
        {
            if (!(Context.Channel is SocketGuildChannel channel)) return;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }
            var role = Context.Guild.EveryoneRole;
            await channel.AddPermissionOverwriteAsync(role, OverwritePermissions.DenyAll(channel)
                .Modify(viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow));
            var builder = new EmbedBuilder()
                .WithDescription("`Channel locked");
            await ReplyAsync(null, false, builder.Build());
            /*
            foreach (var role in Context.Guild.Roles)
            {
                await channel.AddPermissionOverwriteAsync(role, OverwritePermissions.DenyAll(channel)
                    .Modify(viewChannel: PermValue.Allow, readMessageHistory: PermValue.Allow));
            }
            */
        }

        [Command("unlock",true)]
        [Summary("unlock channel")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task UnlockChannel()
        {
            if (!(Context.Channel is SocketGuildChannel channel)) return;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }
            var role = Context.Guild.EveryoneRole;
            await channel.AddPermissionOverwriteAsync(role, OverwritePermissions.InheritAll);
            var builder = new EmbedBuilder()
                .WithDescription("`Channel unlocked");
            await ReplyAsync(null, false, builder.Build());
            /*
            var mutedRole = Context.Guild.Roles.FirstOrDefault(t => t.Name.ToLower().Equals("muted"));
            foreach (var role in Context.Guild.Roles)
            {
                if (mutedRole == role) continue;
                await channel.AddPermissionOverwriteAsync(role, OverwritePermissions.InheritAll);
            }
            */
        }

        [Command("purge")]
        [Summary("Purge message from channel")]
        //[RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int amount)
        {
            if (amount <= 0)
            {
                await Context.Channel.SendMessageAsync("No input");
                return;
            }
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }

            var messages = (await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync()).ToList();
            await ((SocketTextChannel)Context.Channel).DeleteMessagesAsync(messages);

            var message =
                await Context.Channel.SendMessageAsync($"{messages.Count} messages deleted successfully!");

            await Task.Delay(2500);

            await message.DeleteAsync();
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
                            continue;
                        }
                        await roles.ModifyAsync(r => r.Permissions = Utils.MemPermissions);
                    }
                    catch
                    {
                        Console.WriteLine($"{roles} is higher or equals bot's");
                    }
                }
                await Context.Channel.SendMessageAsync("Mention disabled on all non-powered roles");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Permission denied!");
            }
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

                        await ReplyAsync($"Nuking this channel {Context.Channel.Name} in 10s");
                        await Task.Delay(10000);

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

        [Command("inhert")]
        [Summary("Inhert perm from category")]
        [Alias("copy")]
        public async Task InhertCategory()
        {
            if (!(Context.Channel is SocketGuildChannel channel)) return;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }
            var mutedRole = Context.Guild.Roles.FirstOrDefault(t => t.Name.ToLower().Equals("muted"));
            foreach (var role in Context.Guild.Roles)
            {
                if (mutedRole == role) continue;
                await channel.AddPermissionOverwriteAsync(role, OverwritePermissions.InheritAll);
            }
        }

        [Command("newch")]
        [Summary("create a new channel")]
        [Alias("new")]
        public async Task CreateChannel()
        {
            if (!(Context.Channel is SocketGuildChannel channel)) return;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }
            var channelCreation = Context.Guild;
            var builder = new EmbedBuilder()
                .WithTitle("**Choose your option**")
                .WithDescription("1. Create a new channel with a new / existing category\n"
                                 + "2. Create a new channel with no category\n")
                .WithCurrentTimestamp()
                .WithColor(new Color(54, 57, 62));
            await ReplyAndDeleteAsync(null, false, builder.Build(), TimeSpan.FromSeconds(10));
            var response = await NextMessageAsync(true, true, TimeSpan.FromSeconds(7));
            await Context.Channel.DeleteMessageAsync(response);
            if (response == null) return;
            if ((response.ToString() == "1" || response.ToString() == "2"))
            {
                try
                {
                    var channelName = await CheckChannelName();
                    if (channelName == null) return;
                    var topic = await CheckChannelTopic();
                    if (topic == null) return;
                    switch (response.ToString())
                    {
                        case "1":
                            var categoryName = await CheckCategory();
                            if (categoryName == null) 
                            {
                                await ReplyAndDeleteAsync($"This category doesn't exist in this server", false, null, TimeSpan.FromSeconds(7));
                                return;
                            }
                            var categoryNameChecker = Context.Guild.CategoryChannels.FirstOrDefault(
                                c => c.Name == categoryName.ToString().ToLower())?.Id;
                            if (categoryNameChecker != null)
                            {
                                var channelCreated = await channelCreation.CreateTextChannelAsync($"{channelName}",
                                    r => r.CategoryId = categoryNameChecker);
                                await channelCreated.ModifyAsync(r => r.Topic = topic.ToString());
                                await ReplyAndDeleteAsync(
                                    $"`Channel : {channel} created at {DateAndTime.Now} and is in category : {categoryNameChecker}!`",
                                    false, null, TimeSpan.FromSeconds(7));
                            }
                            else
                            {
                                await channelCreation.CreateCategoryChannelAsync($"{categoryName}",
                                    r => r.Position = 0);
                                var categoryCreated = Context.Guild.CategoryChannels.FirstOrDefault(
                                    c => c.Name == categoryName.ToString().ToLower())?.Id;
                                if (categoryCreated == null)
                                {
                                    await ReplyAndDeleteAsync(
                                        "Can't find the category created ? Maybe your server is full!", false, null,
                                        TimeSpan.FromSeconds(7));
                                    return;
                                }
                                var createNewChannel = await channelCreation.CreateTextChannelAsync($"{channelName}",
                                    r => r.CategoryId = categoryCreated);
                                await createNewChannel.ModifyAsync(r => r.Topic = topic.ToString());
                                await ReplyAndDeleteAsync(
                                    $"`Channel : {createNewChannel} created at {DateAndTime.Now} and is in category : {categoryName}`", 
                                    false, null, TimeSpan.FromSeconds(7));
                            }
                            break;
                        case "2":
                            await channelCreation.CreateTextChannelAsync($"{channelName}", r => r.Topic = topic.ToString());
                            await ReplyAndDeleteAsync($"`Channel : {channelName} created at {DateAndTime.Now} !`", false, null,
                                TimeSpan.FromSeconds(7));
                            break;
                        default:
                            await ReplyAndDeleteAsync("Error !", false, null, TimeSpan.FromSeconds(5));
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Logging : Error happened at choice {response} ! at {DateAndTime.Now}!", e);
                }
            }
            else
            {
                await ReplyAsync($"{Context.User.Mention}, command timed out...!");
            }
        }

        [Command("delch")]
        [Summary("Delete a specifics channel")]
        [Alias("delet", "delete")]
        public async Task DeleteChannel()
        {
            if (!(Context.Channel is SocketGuildChannel channel)) return;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }
            var builder = new EmbedBuilder()
                .WithTitle("**Choose your option**")
                .WithDescription("1. Delete channel in a existing category\n"
                                 + "2. Delete category and its child\n"
                                 + "3. Delete category only")
                .WithCurrentTimestamp()
                .WithColor(new Color(54, 57, 62));
            await ReplyAndDeleteAsync(null, false, builder.Build(), TimeSpan.FromSeconds(10));
            var response = await NextMessageAsync(true, true, TimeSpan.FromSeconds(7));
            await Context.Channel.DeleteMessageAsync(response);
            if (response == null) return;
            if ((response.ToString() == "1" || response.ToString() == "2") || response.ToString() == "3")
            {
                try
                {
                    var categoryName = await CheckCategory();
                    if (categoryName == null) return;
                    var categoryNameChecker = Context.Guild.CategoryChannels.FirstOrDefault(
                        c => c.Name == categoryName.ToString().ToLower());
                    if (categoryNameChecker == null)
                    {
                        await ReplyAndDeleteAsync($"This category doesn't exist in this server", false, null, TimeSpan.FromSeconds(7));
                        return;
                    }
                    switch (response.ToString())
                    {
                        //Delete channel in a existing category
                        case "1":
                            var channelChannelExist = await CheckChannelExist();
                            if (channelChannelExist == null)
                            {
                                await ReplyAndDeleteAsync($"This channel doesn't exist in this server", false, null, TimeSpan.FromSeconds(7));
                                return;
                            }
                            else
                            {
                                var channelChecker = Context.Guild.Channels.FirstOrDefault(
                                    c => c.Name == channelChannelExist.ToString().ToLower());
                                if (channelChecker == null) return;
                                await channelChecker.DeleteAsync();
                            }
                            break;
                        //2. Delete category and its child
                        case "2":
                            var child = categoryNameChecker.Channels;
                            foreach (var channelDeletion in child)
                                await channelDeletion.DeleteAsync();
                            await categoryNameChecker.DeleteAsync();
                            await ReplyAndDeleteAsync($"{categoryNameChecker} and its child has been deleted", false, null, TimeSpan.FromSeconds(7));
                            break;
                        //3. Delete category only
                        case "3":
                            await categoryNameChecker.DeleteAsync();
                            await ReplyAndDeleteAsync($"{categoryNameChecker} and has been deleted", false, null, TimeSpan.FromSeconds(7));
                            break;
                    }
                } catch (Exception e) 
                {
                    Console.WriteLine($"{e}");
                }
            }
        }

        private async Task<SocketMessage> CheckChannelExist()
        {
            await ReplyAndDeleteAsync("Name of the channel you want to delete please ?", false, null, TimeSpan.FromSeconds(7));
            var channelName = await NextMessageAsync(true, true, TimeSpan.FromSeconds(7));
            if (channelName == null) return null;
            await Context.Channel.DeleteMessageAsync(channelName);
            return channelName;
        }

        private async Task<SocketMessage> CheckChannelName()
        { 
            await ReplyAndDeleteAsync("Name of the new channel please?", false, null, TimeSpan.FromSeconds(7));
            var channelName = await NextMessageAsync(true, true, TimeSpan.FromSeconds(7)); 
            if (channelName == null) return null; 
            await Context.Channel.DeleteMessageAsync(channelName); 
            return channelName;
        }

        private async Task<SocketMessage> CheckChannelTopic() 
        { 
            await ReplyAndDeleteAsync("Topic of the new channel please?", false, null, TimeSpan.FromSeconds(7)); 
            var channelTopic = await NextMessageAsync(true, true, TimeSpan.FromSeconds(7)); 
            if (channelTopic == null) return null; 
            await Context.Channel.DeleteMessageAsync(channelTopic); 
            return channelTopic;
        }
        private async Task<SocketMessage> CheckCategory()
        {
            await ReplyAndDeleteAsync("Category name please ?", false, null, TimeSpan.FromSeconds(7)); 
            var categoryName = await NextMessageAsync(true, true, TimeSpan.FromSeconds(7)); 
            if (categoryName == null) return null; 
            await Context.Channel.DeleteMessageAsync(categoryName); 
            return categoryName;
        }
    }
}
