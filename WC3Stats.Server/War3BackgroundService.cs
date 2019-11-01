using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace WC3Stats.Server
{
    public class War3BackgroundService : BackgroundService
    {
        private readonly IHubContext<Wc3Hub, IWc3> _hub;

        public War3BackgroundService(IHubContext<Wc3Hub, IWc3> hub)
        {
            _hub = hub;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //var players = await GameMonitor.LookForPlayers();
                var players = new List<Player>
                {
                    new Player("Rookian", Races.Orc, 1, true){PlayerStats = PlayerStats()},
                    new Player("NightEnD", Races.Human, 3, false) {PlayerStats = PlayerStats()},
                    new Player("Feanor", Races.NightElf, 5, false){PlayerStats = PlayerStats()},
                    new Player("ILoveNecropolis", Races.Random, 7, false){PlayerStats = PlayerStats()},

                    new Player("FollowGrubby", Races.Orc, 2, false){PlayerStats = PlayerStats()},
                    new Player("TimonCrab", Races.Undead, 4, false){PlayerStats = PlayerStats()},
                    new Player("RomanticHuman", Races.NightElf, 6, false){PlayerStats = PlayerStats()},
                    new Player("123456789012345", Races.Random, 8, false){PlayerStats = PlayerStats()}
                };

                await _hub.Clients.All.Send(players);
                await Task.Delay(10000);
                //await Task.Yield();
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
}