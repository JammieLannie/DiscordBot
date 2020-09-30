using System;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Nekos.Net;
using Nekos.Net.Endpoints;

namespace DiscordBot.Modules
{
    [Summary(":sa:")]
    internal class NSFW : InteractiveBase
    {
        [Command("boob")]
        [Summary("Get boob nsfw image!!")]
        public async Task NekoBoob()
        {
            if (!isNSFWChannel()) return; //todo
            await BadAtNamming(NsfwEndpoint.Boobs);
        }
        
        
        private async Task BadAtNamming(NsfwEndpoint endpoint)
        {
            try
            {
                var author = Context.User;
                var image = await NekosClient.GetNsfwAsync(endpoint);
                await ReplyAsync(null, false, new EmbedBuilder().WithDescription($"{author.Mention}, here you go '{endpoint.ToString().Replace("_", " ")}'").WithImageUrl(image.FileUrl).Build());
            }
            catch {
                await ReplyAndDeleteAsync("Error !!", false, null, TimeSpan.FromSeconds(5));
            }
        }

        private bool isNSFWChannel() => ((ITextChannel) Context.Channel).IsNsfw;
    }
}