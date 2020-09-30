using System;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Nekos.Net;
using Nekos.Net.Endpoints;
using Nekos.Net.Responses;

namespace DiscordBot.Modules
{
    [Summary(":satellite:")]
    public class Fun : InteractiveBase
    {
        [Command("poke")]
        [Summary("Poke someone or yourself ?")]
        public async Task NekoPoke(IGuildUser user = null)
        {
            var builder = new EmbedBuilder();
            var author = Context.User;
            NekosImage image = await NekosClient.GetSfwAsync(SfwEndpoint.Poke);
            if (user != null && user != author)
            {
                builder.WithDescription($"{author.Mention} has poked {user.Mention}!")
                    .WithImageUrl(image.FileUrl);
                await ReplyAsync(null, false, builder.Build());
            }
            else if (user == author || user == null)
            {
                builder.WithDescription($"{author.Mention} has poked themselves ?")
                    .WithImageUrl(image.FileUrl);
                await ReplyAsync(null, false, builder.Build());
            }
            else
            {
                await ReplyAndDeleteAsync("Error !!", false, null, TimeSpan.FromSeconds(5));
            }
        }

        [Command("cuddle")]
        [Summary("Cuddle someone or yourself ?")]
        public async Task NekoCuddle(IGuildUser user = null)
        {
            var builder = new EmbedBuilder();
            var author = Context.User;
            NekosImage image = await NekosClient.GetSfwAsync(SfwEndpoint.Cuddle);
            if (user != null && user != author)
            {
                builder.WithDescription($"{author.Mention} has cuddled {user.Mention}!")
                    .WithImageUrl(image.FileUrl);
                await ReplyAsync(null, false, builder.Build());
            }
            else if (user == author || user == null)
            {
                builder.WithDescription($"{author.Mention} has cuddled themselves ?")
                    .WithImageUrl(image.FileUrl);
                await ReplyAsync(null, false, builder.Build());
            }
            else
            {
                await ReplyAndDeleteAsync("Error !!", false, null, TimeSpan.FromSeconds(5));
            }
        }

        [Command("pat")]
        [Summary("Pat someone or yourself ?")]
        public async Task NekoPat(IGuildUser user = null)
        {
            var builder = new EmbedBuilder();
            var author = Context.User;
            NekosImage image = await NekosClient.GetSfwAsync(SfwEndpoint.Pat);
            if (user != null && user != author)
            {
                builder.WithDescription($"{author.Mention} has patted {user.Mention}!")
                    .WithImageUrl(image.FileUrl);
                await ReplyAsync(null, false, builder.Build());
            }
            else if (user == author || user == null)
            {
                builder.WithDescription($"{author.Mention} has patted themselves ?")
                    .WithImageUrl(image.FileUrl);
                await ReplyAsync(null, false, builder.Build());
            }
            else
            {
                await ReplyAndDeleteAsync("Error !!", false, null, TimeSpan.FromSeconds(5));
            }
        }

        [Command("tickle")]
        [Summary("Tickle someone or yourself ?")]
        public async Task NekoTickle(IGuildUser user = null)
        {
            var builder = new EmbedBuilder();
            var author = Context.User;
            NekosImage image = await NekosClient.GetSfwAsync(SfwEndpoint.Tickle);
            if (user != null && user != author)
            {
                builder.WithDescription($"{author.Mention} has tickled {user.Mention}!")
                    .WithImageUrl(image.FileUrl);
                await ReplyAsync(null, false, builder.Build());
            }
            else if (user == author || user == null)
            {
                builder.WithDescription($"{author.Mention} has tickled themselves ?")
                    .WithImageUrl(image.FileUrl);
                await ReplyAsync(null, false, builder.Build());
            }
            else
            {
                await ReplyAndDeleteAsync("Error !!", false, null, TimeSpan.FromSeconds(5));
            }
        }

        [Command("hug")]
        [Summary("Hug someone or yourself ?")]
        public async Task NekoHug(IGuildUser user = null)
        {
            var builder = new EmbedBuilder();
            var author = Context.User;
            NekosImage image = await NekosClient.GetSfwAsync(SfwEndpoint.Hug);
            if (user != null && user != author)
            {
                builder.WithDescription($"{author.Mention} has hugged {user.Mention}!")
                    .WithImageUrl(image.FileUrl);
                await ReplyAsync(null, false, builder.Build());
            }
            else if (user == author || user == null)
            {
                builder.WithDescription($"{author.Mention} has hugged themselves ?")
                    .WithImageUrl(image.FileUrl);
                await ReplyAsync(null, false, builder.Build());
            }
            else
            {
                await ReplyAndDeleteAsync("Error !!", false, null, TimeSpan.FromSeconds(5));
            }
        }

        [Command("slap")]
        [Summary("Slap someone or yourself ?")]
        public async Task NekoSlap(IGuildUser user = null)
        {
            var builder = new EmbedBuilder();
            var author = Context.User;
            NekosImage image = await NekosClient.GetSfwAsync(SfwEndpoint.Slap);
            if (user != null && user != author)
            {
                builder.WithDescription($"{author.Mention} has slapped {user.Mention}!")
                    .WithImageUrl(image.FileUrl);
                await ReplyAsync(null, false, builder.Build());
            }
            else if (user == author || user == null)
            {
                builder.WithDescription($"{author.Mention} has slapped themselves ?")
                    .WithImageUrl(image.FileUrl);
                await ReplyAsync(null, false, builder.Build());
            }
            else
            {
                await ReplyAndDeleteAsync("Error !!", false, null, TimeSpan.FromSeconds(5));
            }
        }

        [Command("kiss")]
        [Summary("Kiss someone or yourself ?")]
        public async Task NekoKiss(IGuildUser user = null)
        {
            var builder = new EmbedBuilder();
            var author = Context.User;
            NekosImage image = await NekosClient.GetSfwAsync(SfwEndpoint.Kiss);
            if (user != null && user != author)
            {
                builder.WithDescription($"{author.Mention} has kissed {user.Mention}!")
                    .WithImageUrl(image.FileUrl);
                await ReplyAsync(null, false, builder.Build());
            }
            else if (user == author || user == null)
            {
                builder.WithDescription($"{author.Mention} has kissed themselves ?")
                    .WithImageUrl(image.FileUrl);
                await ReplyAsync(null, false, builder.Build());
            }
            else
            {
                await ReplyAndDeleteAsync("Error !!", false, null, TimeSpan.FromSeconds(5));
            }
        }
    }
}
