using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot.Services
{
    public class LoggingService
    {
        public LoggingService(DiscordSocketClient client, CommandService command)
        {
            client.Log += ClientOnLog;
            command.Log += ClientOnLog;
        }

        private async Task ClientOnLog(LogMessage message)
        {
            if (message.Exception is CommandException cmdException)
            {
                Console.WriteLine($"[CommandHandler/{message.Severity}] {cmdException.Command.Aliases.First()}"
                                  + $" failed to execute in {cmdException.Context.Channel}.");
                Console.WriteLine(cmdException);
            }
            else
            {
                Console.WriteLine($"[General/{message.Severity}] {message}");
            }
        }
    }
}