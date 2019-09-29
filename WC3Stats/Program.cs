using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace WC3Stats
{

    class Program
    {
        static async Task Main()
        {
            Console.SetCursorPosition((Console.WindowWidth - 46) / 2, Console.CursorTop);
            Console.WriteLine("╦ ╦┌─┐┬─┐┌─┐┬─┐┌─┐┌─┐┌┬┐  ╦╦╦  ╔═╗┌┬┐┌─┐┌┬┐┌─┐");
            Console.SetCursorPosition((Console.WindowWidth - 46) / 2, Console.CursorTop);
            Console.WriteLine("║║║├─┤├┬┘│  ├┬┘├─┤├┤  │   ║║║  ╚═╗ │ ├─┤ │ └─┐");
            Console.SetCursorPosition((Console.WindowWidth - 46) / 2, Console.CursorTop);
            Console.WriteLine("╚╩╝┴ ┴┴└─└─┘┴└─┴ ┴└   ┴   ╩╩╩  ╚═╝ ┴ ┴ ┴ ┴ └─┘");


            while (true)
            {
                var gameMonitor = new GameMonitor();
                Console.WriteLine();
                Console.WriteLine("Waiting for game ...");
                var game = await GameMonitor.Start();

                Console.WriteLine("Game started");
                foreach (var teamGroup in game.Players.GroupBy(x => x.Team))
                {
                    Console.WriteLine($"#Team {teamGroup.Key}");
                    foreach (var player in teamGroup.OrderByDescending(x => x.PlayerStats.TeamWinRate))
                    {
                        var playerStats = player.PlayerStats;
                        Console.WriteLine($"{player.Name.PadRight(14)} {player.Race.ToString().PadRight(9)} - Solo: {playerStats.SoloWinRate.ToString(CultureInfo.InvariantCulture).PadRight(4)}% - Team: {playerStats.TeamWinRate.ToString(CultureInfo.InvariantCulture).PadRight(4)}%");
                    }

                    Console.WriteLine();
                }
            }
        }
    }
}
