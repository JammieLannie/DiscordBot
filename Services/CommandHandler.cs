using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace DiscordBot.Services
{
    public class CommandHandler : InitializedService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _service;
        private readonly IConfiguration _config;

        public CommandHandler(IServiceProvider provider, DiscordSocketClient client, CommandService service,
            IConfiguration config)
        {
            _provider = provider;
            _client = client;
            _service = service;
            _config = config;
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            _client.MessageReceived += OnMessageReceived;
            _client.JoinedGuild += OnJoinedGuild;
            _client.ReactionAdded += OnReactionAdded;

            _service.CommandExecuted += OnCommandExecuted;
            _client.Ready += OnReady;
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2,
            SocketReaction arg3)
        {
            if (arg3.MessageId != 762293973137227776) return;

            if (arg3.Emote.Name != "✅") return;

            var role = (arg2 as SocketGuildChannel).Guild.Roles.FirstOrDefault(x => x.Id == 123);
            await (arg3.User.Value as SocketGuildUser).AddRoleAsync(role);
        }

        private async Task OnJoinedGuild(SocketGuild arg)
        {
            await arg.DefaultChannel.SendMessageAsync($"Hello, Ｍｙｏｕｒｉ is here. Thanks for inviting me to your server!");
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;
            if (!message.HasStringPrefix(_config["prefix"], ref argPos) &&
                !message.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(_client, message);
            await _service.ExecuteAsync(context, argPos, _provider);
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (command.IsSpecified && !result.IsSuccess) await context.Channel.SendMessageAsync($"Error: {result}");
        }

        private async Task OnReady()
        {
            await _client.SetGameAsync("with Olivia");
            await _client.SetStatusAsync(status: UserStatus.DoNotDisturb);
        }
    }
}