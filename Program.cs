using System.Threading.Tasks;
using DiscordBot.Modules;

namespace DiscordBot
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Startup.RunAsync(args).Wait();
        }
    }
}