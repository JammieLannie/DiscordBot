using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace DiscordBot.Services
{
    public class CommandHandler
    {
        public static IServiceProvider _provider;
        public static DiscordSocketClient _discord;
        public static CommandService _commands;
        public static IConfigurationRoot _config;

        // Retrieve client and CommandService instance via ctor
        public CommandHandler(DiscordSocketClient discord, CommandService commands, IConfigurationRoot config,
            IServiceProvider provider)
        {
            _provider = provider;
            _config = config;
            _discord = discord;
            _commands = commands;

            _discord.Ready += OnReady;
            _discord.MessageReceived += OnMessageReceived;
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;

            if (msg.Author.IsBot) return;

            var context = new SocketCommandContext(_discord, msg);

            int pos = 0;

            if (msg.HasStringPrefix(_config["prefix"], ref pos) || msg.HasMentionPrefix(_discord.CurrentUser, ref pos))
            {
                var result = await _commands.ExecuteAsync(context, pos, _provider);

                if (!result.IsSuccess)
                {
                    var reason = result.Error;

                    Console.WriteLine(reason);
                }
            }
        }


        private async Task OnReady()
        {
            Console.Write($"Connected as {_discord.CurrentUser.Username}#{_discord.CurrentUser.Discriminator}");
        }
    }
}
