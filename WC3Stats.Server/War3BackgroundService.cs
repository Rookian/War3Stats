using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace WC3Stats.Server
{
    public class War3BackgroundService : BackgroundService
    {
        private readonly IHubContext<Wc3Hub, IWc3> _hub;
        private readonly BackgroundServiceConfiguration _configuration;

        public War3BackgroundService(IHubContext<Wc3Hub, IWc3> hub, BackgroundServiceConfiguration configuration)
        {
            _hub = hub;
            _configuration = configuration;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                Console.SetCursorPosition((Console.WindowWidth - 46) / 2, Console.CursorTop);
                Console.WriteLine("╦ ╦┌─┐┬─┐┌─┐┬─┐┌─┐┌─┐┌┬┐  ╦╦╦  ╔═╗┌┬┐┌─┐┌┬┐┌─┐");
                Console.SetCursorPosition((Console.WindowWidth - 46) / 2, Console.CursorTop);
                Console.WriteLine("║║║├─┤├┬┘│  ├┬┘├─┤├┤  │   ║║║  ╚═╗ │ ├─┤ │ └─┐");
                Console.SetCursorPosition((Console.WindowWidth - 46) / 2, Console.CursorTop);
                Console.WriteLine("╚╩╝┴ ┴┴└─└─┘┴└─┴ ┴└   ┴   ╩╩╩  ╚═╝ ┴ ┴ ┴ ┴ └─┘");
                Console.WriteLine();
                Console.WriteLine();

                if (_configuration.Simulate)
                    Console.WriteLine("Simulation mode active");

                WriteLineRight("Powered by Rookian");
                WriteLineRight("https://github.com/Rookian/War3Stats");
                Console.WriteLine();
            }
            catch { }
            return base.StartAsync(cancellationToken);
        }

        private static void WriteLineRight(string value)
        {
            Console.CursorLeft = Console.BufferWidth - value.Length - 1;
            Console.WriteLine(value);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                List<Player> players;
                if (true)
                {

                    await _hub.Clients.All.GameFound();
                    await Task.Delay(4000, stoppingToken);

                    players = new List<Player>
                    {
                        new Player("Rookian", Races.Orc, 1, true){ PlayerStats = PlayerStats()},
                        new Player("NightEnD", Races.Human, 3, false) { PlayerStats = PlayerStats()},
                        new Player("Feanor", Races.NightElf, 5, false){ PlayerStats = PlayerStats()},
                        new Player("ILoveNecropolis", Races.Random, 7, false){ PlayerStats = PlayerStats()},

                        new Player("FollowGrubby", Races.Orc, 2, false){ PlayerStats = PlayerStats()},
                        new Player("TimonCrab", Races.Undead, 4, false){ PlayerStats = PlayerStats()},
                        new Player("RomanticHuman", Races.NightElf, 6, false){ PlayerStats = PlayerStats()},
                        new Player("123456789012345", Races.Random, 8, false){ PlayerStats = PlayerStats()}
                    };
                    await _hub.Clients.All.Send(players);
                    await Task.Delay(4000, stoppingToken);
                }
                else
                {
                    players = await GameMonitor.LookForPlayers(async () => await _hub.Clients.All.GameFound());
                    await _hub.Clients.All.Send(players);
                    await Task.Delay(100);
                }
            }
        }

        private static PlayerStats PlayerStats()
        {
            var random = new Random();
            return new PlayerStats
            {
                SoloLevel = random.Next(0, 50),
                SoloLosses = random.Next(0, 50),
                SoloWins = random.Next(0, 50),
                TeamLevel = random.Next(0, 50),
                TeamLosses = random.Next(0, 50),
                TeamWins = random.Next(0, 50)
            };
        }
    }

    public class BackgroundServiceConfiguration
    {
        public BackgroundServiceConfiguration(bool simulate)
        {
            Simulate = simulate;
        }

        public bool Simulate { get; }
    }
}