using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;


namespace DiscordBot.Modules
{
    [Summary(":woman_in_motorized_wheelchair:")]
    public class General : ModuleBase
    {
        
        private readonly CommandService _commands;
        private readonly IConfiguration _config;

        public General(CommandService commands, IConfiguration config)
        {
            _commands = commands;
            _config = config;
        }
        

        [Command("ping", true)]
        [Summary("Get bot latency in ms")]
        public async Task Ping()
        {
            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);
            await Context.Channel.SendMessageAsync("Bonk bonk the bot with " + Context.Message.CreatedAt.Millisecond +
                                                   "ms");
        }
        
        [Command("help", true)]
        [Alias("help me")]
        public async Task Help([Remainder] string arg = null)
        {
            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);
            var builder = new EmbedBuilder();
            builder.WithTitle("Help me please")
                .WithDescription($"You can use `{_config["prefix"]}help <cmd/catalog>!`")
                .WithFooter(_commands.Commands.Count() + " commands", Context.Client.CurrentUser.GetAvatarUrl());
            if (arg == null)
            {
                foreach (var module in _commands.Modules)
                    builder.AddField($"{module.Summary.ToLower()} {module.Name}", $"{module.Commands.Count} commands",
                        true);
                await Context.Channel.SendMessageAsync(null, false, builder.Build());
                return;
            }

            arg = arg.ToLower();
            var m = _commands.Modules.FirstOrDefault(c => c.Name.ToLower().Equals(arg));
            if (m == null)
            {
                var cmd = _commands.Commands.FirstOrDefault(c => c.Name.ToLower().Equals(arg));
                if (cmd == null)
                {
                    await Context.Channel.SendMessageAsync(
                        $"Not found command/catalog `{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(arg)}`");
                    return;
                }

                var desc = "Description: " + (cmd.Summary ?? "No description");
                var parameter = cmd.Parameters.Aggregate<ParameterInfo, string>(null,
                    (current, parameterInfo) =>
                        $"{(current == null ? null : current + "\n")}'{parameterInfo.Name}' [{parameterInfo.Type.Name}] - {parameterInfo.Summary ?? "No description"}");
                builder.WithDescription(desc).WithTitle($"Help for: {cmd.Name}");
                if (!string.IsNullOrEmpty(parameter)) builder.AddField("Parameters", parameter);
                await Context.Channel.SendMessageAsync(null, false, builder.Build());
                return;
            }

            foreach (var cmd in m.Commands)
                builder.AddField($"{_config["prefix"]}{cmd.Name}", cmd.Summary ?? "No description", true)
                    .WithFooter(m.Commands.Count + " commands", Context.Client.CurrentUser.GetAvatarUrl());
            await Context.Channel.SendMessageAsync(null, false, builder.Build());
        }
        
        [Command("info", true)]
        [Summary("Get your information")]
        public async Task Info(IGuildUser user = null)
        {
            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);
            user = (IGuildUser) (user ?? Context.User);
            var builder = new EmbedBuilder();
            builder.WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                .WithDescription("Basic information about yourself!")
                .WithColor(Color.DarkTeal)
                .AddField("User ID: ", user.Id, true)
                .AddField("Created at ", user.CreatedAt.ToString("dd/MM/yyyy"), true)
                .AddField("Joined at", ((SocketGuildUser) user).JoinedAt?.ToString("dd/MM/yyyy"), true)
                .AddField("Roles", string.Join(", ", ((SocketGuildUser) user).Roles.Select(x => x.Mention)))
                .WithCurrentTimestamp()
                .WithAuthor(Context.User);
            await Context.Channel.SendMessageAsync(null, false, builder.Build());
        }

        [Command("bot", true)]
        [Summary("Get bot data")]
        public async Task BotInfo()
        {
            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);
            var builder = new EmbedBuilder();
            builder.WithTitle("Bot status")
                .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())
                .AddField("CPU usage", Utils.GetCpuCounter() + " %", true)
                .AddField("Ram usage", Utils.RamUsage() + " %", true)
                .AddField("Bot ID", Context.Client.CurrentUser.Id)
                .AddField("Owner", "**Olivia#8888** (ID: 247742975608750090)")
                .WithCurrentTimestamp()
                .WithColor(34, 53, 67);
            await Context.Channel.SendMessageAsync(null, false, builder.Build());
        }

        [Command("8ball", true)]
        [Summary("8ball like command")]
        public async Task EightBall([Remainder] string message)
        {
            var builder = new EmbedBuilder();
            builder.WithTitle("8Ball game")
                .WithCurrentTimestamp()
                .WithColor(Color.Green)
                .WithAuthor(Context.User)
                .WithColor(new Color(0, 255, 0))
                .AddField("Your question", $"{message}");
            var replies = new []
            {
                "Yes",
                "No",
                "Maybe...",
                "I don't know"
            };
            if (string.IsNullOrEmpty(message))
                builder.WithDescription("8Ball can't answer if you don't ask!!");
            else
                builder.AddField("Answer", $"{replies[new Random().Next(replies.Length - 1)]}");
            await Context.Channel.SendMessageAsync(null, false, builder.Build());
        }

        [Command("reddit", true)]
        [Alias("reddit")]
        [Summary("Get subreddit random post")]
        public async Task RandomPost(string subreddit = null)
        {
            var client = new HttpClient();
            var result =
                await client.GetStringAsync($"https://reddit.com/r/{subreddit}/random.json?limit=1");
            var builder = new EmbedBuilder();
            if (!result.StartsWith("["))
            {
                await Context.Channel.SendMessageAsync("This subreddit doesn't exist!");
                return;
            }

            var arr = JArray.Parse(result);
            var post = JObject.Parse(arr[0]["data"]["children"][0]["data"].ToString());

            builder.WithImageUrl(post["url"].ToString())
                .WithColor(new Color(33, 176, 252))
                .WithTitle(post["title"].ToString())
                .WithUrl("https://reddit.com" + post["permalink"])
                .WithFooter($"🗨️ {post["num_comments"]} ⬆️ {post["ups"]}");
            await Context.Channel.SendMessageAsync(null, false, builder.Build());
        }

        [Command("say", true)]
        [Description("Repeat what user type")]
        [Summary("Make bot says")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Say([Remainder] string message)
        {
            await Context.Channel.DeleteMessageAsync(Context.Message);
            await Context.Channel.SendMessageAsync(message);
        }

        [Command("server", true)]
        [Summary("Get information about this server")]
        public async Task Server()
        {
            var builder = new EmbedBuilder();
            await Context.Channel.DeleteMessageAsync(Context.Message).ConfigureAwait(false);
            builder.WithThumbnailUrl(Context.Guild.IconUrl)
                .WithDescription("Basic information about the current server.")
                .WithTitle($"{Context.Guild.Name} Information")
                .WithColor(new Color(33, 176, 252))
                .AddField("Created at", Context.Guild.CreatedAt.ToString("dd/MM/yyyy"), true)
                .AddField("Member count", ((SocketGuild) Context.Guild).MemberCount + " members", true)
                .AddField("Online users",
                    ((SocketGuild) Context.Guild).Users.Count(x => x.Status != UserStatus.Offline) +
                    " members", true);
            await Context.Channel.SendMessageAsync(null, false, builder.Build());
        }
    }
}