using System;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Nekos.Net;
using Nekos.Net.Endpoints;

namespace DiscordBot.Modules
{
    [Summary(":satellite:")]
    public class Fun : InteractiveBase
    {

        [Command("poke")]
        [Summary("Poke someone or yourself ?")]
        public async Task NekoPoke(IGuildUser user = null)
        {
            await SendFunCmd(SfwEndpoint.Poke, user);
        }
        
        [Command("cuddle")]
        [Summary("Cuddle someone or yourself ?")]
        public async Task NekoCuddle(IGuildUser user = null)
        {
            await SendFunCmd(SfwEndpoint.Cuddle, user);
        }

        [Command("pat")]
        [Summary("Pat someone or yourself ?")]
        public async Task NekoPat(IGuildUser user = null)
        {
            await SendFunCmd(SfwEndpoint.Pat, user);
        }

        [Command("tickle")]
        [Summary("Tickle someone or yourself ?")]
        public async Task NekoTickle(IGuildUser user = null)
        {
            await SendFunCmd(SfwEndpoint.Tickle, user);
        }

        [Command("hug")]
        [Summary("Hug someone or yourself ?")]
        public async Task NekoHug(IGuildUser user = null)
        {
            await SendFunCmd(SfwEndpoint.Hug, user);
        }

        [Command("slap")]
        [Summary("Slap someone or yourself ?")]
        public async Task NekoSlap(IGuildUser user = null)
        {
            await SendFunCmd(SfwEndpoint.Slap, user);
        }

        [Command("kiss")]
        [Summary("Kiss someone or yourself ?")]
        public async Task NekoKiss(IGuildUser user = null)
        {
            await SendFunCmd(SfwEndpoint.Kiss, user);
        }

        private async Task SendFunCmd(SfwEndpoint endpoint, IUser user)
        {
            try
            {
                var author = Context.User;
                var image = await NekosClient.GetSfwAsync(endpoint);
                var fag = endpoint.ToString().Replace("_", " ");
                await ReplyAsync(null, false, new EmbedBuilder().WithDescription(user == null || author == user ? $"{author.Mention} has {fag} themselves ?" : $"{author.Mention} has {fag} {user.Mention}!").WithImageUrl(image.FileUrl).Build());
            }
            catch {
                await ReplyAndDeleteAsync("Error !!", false, null, TimeSpan.FromSeconds(5));
            }
        }
    }
}