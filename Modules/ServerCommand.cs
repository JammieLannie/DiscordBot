using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
namespace DiscordBot.Modules
{
    public class ServerCommand : InteractiveBase
    {
        [Summary(":eyes:")]
        public class Interaction : InteractiveBase
        {
            [Command("Nuke", RunMode = RunMode.Async)]
            [Description("Clone channel and create new one")]
            [Summary("Nuke a channel")]
            [RequireBotPermission(GuildPermission.Administrator)]
            [RequireUserPermission(GuildPermission.Administrator)]

            public async Task NukeChannel()
            {
                await ReplyAndDeleteAsync("Are you sure you want to nuke this channel ?",
                    timeout: TimeSpan.FromSeconds(15));
                var response = await NextMessageAsync();
                if (response != null)
                {
                    if (response.ToString().ToLower().Equals("yes"))
                    {
                        var channel = Context.Channel;

                        var oldChannel = ((ITextChannel)channel);

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

                        return;
                    }
                    else if (response.ToString().ToLower().Equals("no"))
                    {
                        await ReplyAsync($"Nuke cancelled on {Context.Channel.Name}");
                        return;
                    }
                }
                else
                {
                    await ReplyAsync($"{Context.User.Mention}, command timed out...");
                }
            }

        }
    }
}
