using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
        [Command("create")]
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
                .WithDescription("`Channel locked`");
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
                .WithDescription("`Channel unlocked`");
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

        [Command("runlock", true)]
        [Summary("unlock channel")]
        [Alias("rul")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task UnlockChannelRemote(SocketGuildChannel channel)
        {
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }
            var role = Context.Guild.EveryoneRole;
            await channel.AddPermissionOverwriteAsync(role, OverwritePermissions.InheritAll);
            var builder = new EmbedBuilder()
                .WithDescription($"`Channel {channel} unlocked`");
            await ReplyAsync(null, false, builder.Build());

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
                await ReplyAndDeleteAsync($"{Context.User.Mention}, Which channel would you like to nuke ?",
                    timeout: TimeSpan.FromSeconds(15));
                var nukeChannel = await NextMessageAsync(timeout: TimeSpan.FromSeconds(10));
                if (nukeChannel != null)
                {
                    var test = nukeChannel.ToString().Replace("<", "").Replace("#", "").Replace(">", "");
                    var channelId = Convert.ToUInt64(test);
                    var checkChannel = Context.Guild.Channels.FirstOrDefault(
                        c => c.Id == channelId);
                    if (checkChannel == null) return;
                    var oldChannel = (ITextChannel)checkChannel;
                    var guild = Context.Guild;
                    await ReplyAndDeleteAsync($"Are you sure you want to nuke <#{checkChannel.Id}> ?",
                        timeout: TimeSpan.FromSeconds(10));
                    var response = await NextMessageAsync(timeout: TimeSpan.FromSeconds(10));
                    if (response != null)
                    {
                        if (response.ToString().ToLower().Equals("yes"))
                        {
                            await ReplyAndDeleteAsync($"Nuking channel <#{checkChannel.Id}> in 10s", timeout: TimeSpan.FromSeconds(10));
                            await Task.Delay(10000);
                            await guild.CreateTextChannelAsync($"{checkChannel.Name}", newChannel =>
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
                            await ReplyAsync($"Nuke cancelled on {checkChannel.Id}");
                        }
                        else
                        {
                            await ReplyAsync($"{Context.User.Mention}, command timed out...");
                        }
                    }
                }
                else
                {
                    await ReplyAsync($"{Context.User.Mention}, command timed out...");
                }
            }
        }
        /*
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
        */

        [Command("tempvc")]
        [Summary("create a tempvc voice channel")]
        [Alias("tvc")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task CreateTempVoiceChannel()
        {
            await Context.Message.DeleteAsync();
            if (!(Context.Channel is SocketGuildChannel channel)) return;
            if (!(Context.User is SocketGuildUser userSend)
                || !userSend.GuildPermissions.ManageChannels)
            {
                await Utils.SendInvalidPerm(Context.User, Context.Channel);
                return;
            }
            var guild = Context.Guild;
            SocketMessage voiceChannelName = await CheckVoiceChannelName();
            if (voiceChannelName == null) return;
            var categoryName = await CheckCategory();
            if (categoryName == null) return;
            var categoryNameChecker = guild.CategoryChannels.FirstOrDefault(
                c => c.Name == categoryName.ToString().ToLower())?.Id;
            if (categoryNameChecker != null)
            {
                var createNewVoiceChannelCategory = await guild.CreateVoiceChannelAsync($"{voiceChannelName}",
                    r => r.CategoryId = categoryNameChecker);
                await createNewVoiceChannelCategory.ModifyAsync(r => {
                    r.UserLimit = 1;
                    r.Position = 0;
                    r.Bitrate = 96000;
                });
                var voiceIdCategory = createNewVoiceChannelCategory.Id;
                var tempVoiceChannelCategory = Context.Guild.VoiceChannels.
                    FirstOrDefault(prop => prop.Id == voiceIdCategory);
                if (tempVoiceChannelCategory == null) return;
                await ReplyAndDeleteAsync($"`Voice channel [{tempVoiceChannelCategory}] created, you have 20 secs to join!`",
                    false, null, TimeSpan.FromSeconds(10));
                await Task.Delay(20000);
                do
                {
                } while (tempVoiceChannelCategory.Users.Count != 0);
                await tempVoiceChannelCategory.DeleteAsync();
            }
            else
            {
                var tempCategoryChannel = await guild.CreateCategoryChannelAsync($"{categoryName}",
                    r => r.Position = 0);
                categoryNameChecker = guild.CategoryChannels.FirstOrDefault(
                    c => c.Name == categoryName.ToString().ToLower())?.Id;
                if (categoryNameChecker == null) return;
                var createNewChannel = await guild.CreateVoiceChannelAsync($"{voiceChannelName}",
                    r => r.CategoryId = categoryNameChecker);
                await createNewChannel.ModifyAsync(r => {
                    r.UserLimit = 1;
                    r.Position = 0;
                    r.Bitrate = 96000;
                });
                var voiceId = createNewChannel.Id;
                var tempVoiceChannel = Context.Guild.VoiceChannels.
                    FirstOrDefault(prop => prop.Id == voiceId);
                if (tempVoiceChannel == null) return;
                await ReplyAndDeleteAsync($"`Voice channel [{tempVoiceChannel}] created, you have 20 secs to join!`",
                    false, null, TimeSpan.FromSeconds(10));
                await Task.Delay(20000);
                do
                {
                } while (tempVoiceChannel.Users.Count != 0);
                await tempVoiceChannel.DeleteAsync();
                await tempCategoryChannel.DeleteAsync();
            }
        }

        /*
        [Command("connect")]
        public async Task startConnect()
        {
            await ConnectAudio();
        }


        public async Task<IAudioClient> ConnectAudio()
        {
            SocketGuildUser user = Context.User as SocketGuildUser; // Get the user who executed the command
            IVoiceChannel channel = user.VoiceChannel;

            bool shouldConnect = false;

            if (channel == null) // Check if the user is in a channel
            {
                await Context.Message.Channel.SendMessageAsync("Please join a voice channel first.");
            }
            else
            {
                var clientUser = await Context.Channel.GetUserAsync(Context.Client.CurrentUser.Id); // Find the client's current user (I.e. this bot) in the channel the command was executed in
                if (clientUser != null)
                {
                    if (clientUser is IGuildUser bot) // Cast the client user so we can access the VoiceChannel property
                    {
                        if (bot.VoiceChannel == null)
                        {
                            Console.WriteLine("Bot is not in any channels");
                            shouldConnect = true;
                        }
                        else if (bot.VoiceChannel.Id == channel.Id)
                        {
                            Console.WriteLine($"Bot is already in requested channel: {bot.VoiceChannel.Name}");
                        }
                        else
                        {
                            Console.WriteLine($"Bot is in channel: {bot.VoiceChannel.Name}");
                            shouldConnect = true;
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Unable to find bot in server: {Context.Guild.Name}");
                }
            }

            return (shouldConnect ? await channel.ConnectAsync() : null); // Return the IAudioClient or null if there was no need to connect
        }
        */

        [Command("newch")]
        [Summary("create a new channel")]
        [Alias("new")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task CreateChannel()
        {
            await Context.Message.DeleteAsync();
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
            if (response == null) return;
            await Context.Channel.DeleteMessageAsync(response);
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
        [Summary("Delete a specifics channel / categories")]
        [Alias("delet", "delete")]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task DeleteChannel()
        {
            await Context.Message.DeleteAsync();
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
            if (response == null) return;
            await Context.Channel.DeleteMessageAsync(response);
            if ((response.ToString() == "1" || response.ToString() == "2") || response.ToString() == "3")
            {
                try
                {
                    switch (response.ToString())
                    {
                        case "1":
                            var channelChannelExist = await CheckChannelExist();
                            if (channelChannelExist == null)
                                await ReplyAndDeleteAsync($"This channel doesn't exist in this server", false, null, TimeSpan.FromSeconds(7));
                            else
                            {
                                var channelChecker = Context.Guild.Channels.FirstOrDefault(
                                    c => c.Name == channelChannelExist.ToString().ToLower());
                                if (channelChecker == null) return;
                                await channelChecker.DeleteAsync();
                                await ReplyAndDeleteAsync($"{channelChecker} has been deleted", false, null, TimeSpan.FromSeconds(7));
                            }
                            break;
                        case "2":
                            var categoryNameChecker = await CategoryNameChecker();
                            if (categoryNameChecker == null)
                            {
                                await ReplyAndDeleteAsync($"This category doesn't exist in this server", false, null, TimeSpan.FromSeconds(7));
                                return;
                            }
                            var child = categoryNameChecker.Channels;
                            foreach (var channelDeletion in child)
                                await channelDeletion.DeleteAsync();
                            await categoryNameChecker.DeleteAsync();
                            await ReplyAndDeleteAsync($"{categoryNameChecker} and its child has been deleted", false, null, TimeSpan.FromSeconds(7));
                            break;
                        case "3":
                            categoryNameChecker = await CategoryNameChecker();
                            if (categoryNameChecker == null)
                            {
                                await ReplyAndDeleteAsync($"This category doesn't exist in this server", false, null, TimeSpan.FromSeconds(7));
                                return;
                            }
                            await categoryNameChecker.DeleteAsync();
                            await ReplyAndDeleteAsync($"{categoryNameChecker} has been deleted", false, null, TimeSpan.FromSeconds(7));
                            break;
                    }
                } catch (Exception e) 
                {
                    Console.WriteLine($"{e}");
                }
            }
        }

        private async Task<SocketCategoryChannel> CategoryNameChecker()
        {
            var categoryName = await CheckCategory();
            var categoryNameChecker = Context.Guild.CategoryChannels.FirstOrDefault(
                c => c.Name == categoryName.ToString().ToLower());
           return categoryNameChecker;
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
        private async Task<SocketMessage> CheckVoiceChannelName()
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
