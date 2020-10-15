using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Modules;
using DiscordBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiscordBot
{
    internal class Program
    {
        static async Task Main()
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration(x =>
                {
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsetting.json", false, true)
                        .Build();

                    x.AddConfiguration(configuration);
                })
                .ConfigureLogging(x =>
                {
                    x.AddConsole();
                    x.SetMinimumLevel(LogLevel.Debug); // Defines what kind of information should be logged (e.g. Debug, Information, Warning, Critical) adjust this to your liking
                })
                .ConfigureDiscordHost<DiscordSocketClient>((context, config) =>
                {
                    config.SocketConfig = new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Debug, // Defines what kind of information should be logged from the API (e.g. Verbose, Info, Warning, Critical) adjust this to your liking
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = 1000,
                    };

                    config.Token = context.Configuration["token"];
                })
                .UseCommandService((context, config) =>
                {
                    config = new CommandServiceConfig()
                    {
                        CaseSensitiveCommands = false,
                        LogLevel = LogSeverity.Debug
                    };
                })
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddHostedService<CommandHandler>()
                        .AddSingleton<InteractiveService>();
                        
                })
                .UseConsoleLifetime();

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}