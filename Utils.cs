using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordBot
{
    public class Utils
    {
        public static async Task SendInvalidPerm(IUser user, IMessageChannel channel)
        {
            var builder = new EmbedBuilder()
                .WithTitle("Logged Information")
                .AddField("Command issuer", $"{user.Mention}")
                .WithDescription(
                    $"{user.Mention} does not have the permission to do that.")
                .WithFooter($"{user.Username}", user.GetAvatarUrl())
                .WithCurrentTimestamp()
                .WithColor(new Color(54, 57, 62));
            await channel.SendMessageAsync(null, false, builder.Build());
        }

        public static bool CanInteractRole(SocketGuildUser user, SocketRole role)
        {
            return user.Roles.Select(r => r.Position).Prepend(0).Max() > role.Position;
        }
        public static GuildPermissions? AdminPermissions()
        {
            var adminPermissions = new GuildPermissions(
                true, true, true, true, true, true, true, true,
                true, true, true, true, true, true, true, true,
                true, true, true, true, true, true, true, false,
                true, true, true, true, false, false);
            return adminPermissions;
        }

        public static GuildPermissions? ModPermissions()
        {
            var modPermissions = new GuildPermissions(
                true, true, false, false, true, false, true, true,
                true, true, true, true, true, true, true, true,
                true, true, true, true, true, true, true, false,
                true, true, true, true, false, false);
            return modPermissions;
        }
        public static GuildPermissions? MemPermissions()
        {
            var modPermissions = new GuildPermissions(
                true, false, false, false, false, false, true, false,
                true, true, true, false, true, true, true, false,
                true, true, true, false, false, false, true, false,
                true, true, false, false, false, false);
            return modPermissions;
        }
    }
}