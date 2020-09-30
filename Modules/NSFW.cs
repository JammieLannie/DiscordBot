using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Nekos.Net;
using Nekos.Net.Endpoints;
using Nekos.Net.Responses;

namespace DiscordBot.Modules
{
    [Summary(":sa:")]
    class NSFW : InteractiveBase
    {
        private readonly EmbedBuilder _builder = new EmbedBuilder();

        [Command("boob")]
        [Summary("Get boob nsfw image!!")]
        public async Task NekoBoob()
        {
            await Context.Channel.DeleteMessageAsync(Context.Message);

            if (!(Context.User is SocketGuildUser user)) return;



            NekosImage image = await NekosClient.GetNsfwAsync(NsfwEndpoint.Boobs);
            _builder.WithImageUrl(image.FileUrl);
            await ReplyAsync(null, false, _builder.Build());
        }


    }
}
