using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;

namespace DiscordBot.Modules
{
    public class General : ModuleBase
    {
        [Command("Ping")]
        public async Task Ping()
        {
            var message = Context.Message;

            await Context.Channel.DeleteMessageAsync(message).ConfigureAwait(false);

            await Context.Channel.SendMessageAsync($"Bonk bonk the bot with "+Context.Message.CreatedAt.Millisecond+"ms");
        }

        [Command("info")]
        public async Task Info()
        {
            var message = Context.Message;

            await Context.Channel.DeleteMessageAsync(message).ConfigureAwait(false);

            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl())
                .WithDescription("Basic information about yourself!")
                .WithColor(Color.DarkTeal)
                .AddField("User ID: ", Context.User.Id, true)
                .AddField("Created at ", Context.User.CreatedAt.ToString("dd/MM/yyyy"), true)
                .AddField("Joined at", (Context.User as SocketGuildUser).JoinedAt.Value.ToString("dd/MM/yyyy"), true)
                .AddField("Roles", string.Join(" , ", (Context.User as SocketGuildUser).Roles.Select(x => x.Mention)))
                .WithCurrentTimestamp()
                .WithAuthor(Context.User);
            var embed = builder.Build();

            await Context.Channel.SendMessageAsync(null, false, embed);

        }

        [Command("dinfo")]
        public async Task dInfo(IGuildUser user)
        {
            var message = Context.Message;

            await Context.Channel.DeleteMessageAsync(message).ConfigureAwait(false);

            var builder = new EmbedBuilder()
                .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithDescription($"Basic information about {user}!")
                .WithColor(Color.DarkTeal)
                .AddField("User ID: ", user, true)
                .AddField("Created at ", user.CreatedAt.ToString("dd/MM/yyyy"), true)
                .AddField("Joined at", user.JoinedAt.Value.ToString("dd/MM/yyyy"), true)
                .AddField("Roles", string.Join(" , ", (user as SocketGuildUser).Roles.Select(x => x.Mention)))
                .WithCurrentTimestamp()
                .WithAuthor(Context.User);
            var embed = builder.Build();

            await Context.Channel.SendMessageAsync(null, false, embed);

        }

        [Command("mech")]
        [Alias("reddit")]
        public async Task MechanicalKeyboards(string subreddit = null)
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync($"https://reddit.com/r/{subreddit ?? "MechanicalKeyboards"}/random.json?limit=1");
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
                .WithUrl("https://reddit.com" + post["permalink"].ToString())
                .WithFooter($"🗨️ {post["num_comments"]} ⬆️ {post["ups"]}");
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }

        [Command("say")]
        [Description("Repeat what user type")]
        public async Task Say(string message)
        { 
            message = Context.Message.Content;

            var array = message.Split(' ');

            var rest = array.Concat(array.Skip(1));

            Console.WriteLine(message);

            Console.WriteLine(array);

            await Context.Channel.DeleteMessageAsync(Context.Message);

            var repeat = Context.Message.Content;
            await Context.Channel.SendMessageAsync($"{rest}");

        }

        [Command("server")]
        public async Task Server()
        {
            var message = Context.Message;

            await Context.Channel.DeleteMessageAsync(message).ConfigureAwait(false);

            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithDescription("Basic information about the current server.")
                .WithTitle($"{Context.Guild.Name} Information")
                .WithColor(new Color(33, 176, 252))
                .AddField("Created at", Context.Guild.CreatedAt.ToString("dd/MM/yyyy"), true)
                .AddField("Member count", (Context.Guild as SocketGuild).MemberCount + " members", true)
                .AddField("Online users", (Context.Guild as SocketGuild).Users.Where(x => x.Status != UserStatus.Offline).Count() + " members", true);
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }

    }

}

