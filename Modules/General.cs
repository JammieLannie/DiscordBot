using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;

namespace DiscordBot.Modules
{
    public class General : ModuleBase
    {
        [Command("Ping", true)]
        public async Task Ping()
        {
            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);
            await Context.Channel.SendMessageAsync("Bonk bonk the bot with " + Context.Message.CreatedAt.Millisecond + "ms");
        }

        [Command("info", true)]
        public async Task Info()
        {
            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);

            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl())
                .WithDescription("Basic information about yourself!")
                .WithColor(Color.DarkTeal)
                .AddField("User ID: ", Context.User.Id, true)
                .AddField("Created at ", Context.User.CreatedAt.ToString("dd/MM/yyyy"), true)
                .AddField("Joined at", ((SocketGuildUser) Context.User).JoinedAt?.ToString("dd/MM/yyyy"), true)
                .AddField("Roles", string.Join(" , ", ((SocketGuildUser) Context.User).Roles.Select(x => x.Mention)))
                .WithCurrentTimestamp()
                .WithAuthor(Context.User);
            var embed = builder.Build();

            await Context.Channel.SendMessageAsync(null, false, embed);
        }

        [Command("dinfo", true)]
        public async Task dInfo(IGuildUser user)
        {
            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);

            var builder = new EmbedBuilder()
                .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithDescription($"Basic information about {user}!")
                .WithColor(Color.DarkTeal)
                .AddField("User ID: ", user, true)
                .AddField("Created at ", user.CreatedAt.ToString("dd/MM/yyyy"), true)
                .AddField("Joined at", user.JoinedAt?.ToString("dd/MM/yyyy"), true)
                .AddField("Roles", string.Join(" , ", ((SocketGuildUser) user).Roles.Select(x => x.Mention)))
                .WithCurrentTimestamp()
                .WithAuthor(Context.User);
            var embed = builder.Build();

            await Context.Channel.SendMessageAsync(null, false, embed);
        }

        [Command("mech", true), Alias("reddit")]
        public async Task MechanicalKeyboards(string subreddit)
        {
            var client = new HttpClient();
            var result =
                await client.GetStringAsync(
                    $"https://reddit.com/r/{subreddit ?? "MechanicalKeyboards"}/random.json?limit=1");
            if (!result.StartsWith("["))
            {
                await Context.Channel.SendMessageAsync("This subreddit doesn't exist!");
                return;
            }

            JArray arr = JArray.Parse(result);
            JObject post = JObject.Parse(arr[0]["data"]["children"][0]["data"].ToString());

            var builder = new EmbedBuilder()
                .WithImageUrl(post["url"].ToString())
                .WithColor(new Color(33, 176, 252))
                .WithTitle(post["title"].ToString())
                .WithUrl("https://reddit.com" + post["permalink"])
                .WithFooter($"🗨️ {post["num_comments"]} ⬆️ {post["ups"]}");
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }

        [Command("say", true)]
        [Description("Repeat what user type")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Say([Remainder]string message) {
            await Context.Channel.DeleteMessageAsync(Context.Message);
            await Context.Channel.SendMessageAsync(message);
        }

        [Command("server", true)]
        public async Task Server()
        {
            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);

            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithDescription("Basic information about the current server.")
                .WithTitle($"{Context.Guild.Name} Information")
                .WithColor(new Color(33, 176, 252))
                .AddField("Created at", Context.Guild.CreatedAt.ToString("dd/MM/yyyy"), true)
                .AddField("Member count", ((SocketGuild) Context.Guild).MemberCount + " members", true)
                .AddField("Online users",
                    ((SocketGuild) Context.Guild).Users.Count(x => x.Status != UserStatus.Offline) +
                    " members", true);
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }
    }
}