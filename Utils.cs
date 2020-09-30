using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DiscordBot
{
    public class Utils
    {
        private static readonly Random random = new Random();

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        public static float InstalledMemory()
        {
            long memKb;
            GetPhysicallyInstalledSystemMemory(out memKb);
            float memoryInMb = (float)memKb / 1024;
            return memoryInMb;
        }

        public static float RamUsage()
        {
            float freeMemory = GetRamCounter();
            float totalMemory = InstalledMemory();
            float usedMemory = totalMemory - freeMemory;
            float memUsage = (usedMemory / totalMemory) * 100;
            return memUsage;
        }

        public static float GetCpuCounter()
        {
            PerformanceCounter cpuCounter = new PerformanceCounter
            {
                CategoryName = "Processor",
                CounterName = "% Processor Time",
                InstanceName = "_Total"
            };
            // will always start at 0
            float firstValue = cpuCounter.NextValue();
            System.Threading.Thread.Sleep(1000);
            // now matches task manager reading
            float cpuValue = cpuCounter.NextValue();
            return cpuValue;
        }

        public static float GetRamCounter()
        {
            PerformanceCounter memCounter = new PerformanceCounter
            {
                CategoryName = "Memory",
                CounterName = "Available MBytes"
            };
            // will always start at 0
            float firstValue = memCounter.NextValue();
            System.Threading.Thread.Sleep(1000);
            // now matches task manager reading
            float ramValue = memCounter.NextValue();
            return ramValue;
        }

        public static bool ChannelIsNsfw(ITextChannel channel)
        {
            return channel.IsNsfw;
        }
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

        public static int GetUserPos(SocketGuildUser u1)
        {
            return u1.Roles.Select(r => r.Position).Prepend(0).Max();
        }

        public static bool CanInteractRole(SocketGuildUser user, SocketRole role)
        {
            return user.Roles.Select(r => r.Position).Prepend(0).Max() > role.Position;
        }

        public static bool CanInteractUser(SocketGuildUser u1, SocketGuildUser u2)
        {
            return GetUserPos(u1) > GetUserPos(u2);
        }
        public static string GetRandomAlphaNumeric()
        {
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(chars.Select(c => chars[random.Next(chars.Length)]).Take(8).ToArray());
        }


        public static readonly GuildPermissions ModPermissions = new GuildPermissions(
            true, true, false, false, true, false, true, true,
            true, true, true, true, true, true, true, true,
            true, true, true, true, true, true, true, false,
            true, true, true, true);

        public static readonly GuildPermissions MemPermissions = new GuildPermissions(
            true, false, false, false, false, false, true, false,
            true, true, true, false, true, true, true, false,
            true, true, true, false, false, false, true, false,
            true, true);

        public static readonly GuildPermissions MutedPermissions = new GuildPermissions(
            false, false, false, false, false, false, false, false,
            true, false, false, false, false, false, true, false,
            false, false, false, false, false, false, true, false,
            false, false);

    }
}