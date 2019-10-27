using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PacketDotNet;
using SharpPcap;

namespace WC3Stats
{
    public static class GameMonitor
    {
        public static async Task<List<Player>> LookForPlayers()
        {
            var players = new List<Player>();
            var device = IPAddress.Parse("5.42.181.0").GetDefaultCaptureDevice();
            device.Open(DeviceMode.Promiscuous, 1000);
            device.Filter = "ip and tcp";


            var stopwatch = new Stopwatch();
            var playerHandlers = new List<IPlayerHandler>() { new OpponentHandler(), new MyPlayerHandler() };
            var gameStarted = false;
            while (true)
            {
                var rawCapture = device.GetNextPacket();
                if (rawCapture == null) continue;

                var packet = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                if (!(packet.PayloadPacket.PayloadPacket is TcpPacket tcpPacket)) continue;
                if (!tcpPacket.IsInIpRange()) continue;


                foreach (var playerHandler in playerHandlers)
                {
                    if (playerHandler.Accepts(tcpPacket.Bytes))
                    {
                        if (!gameStarted)
                        {
                            gameStarted = true;
                            stopwatch.Start();
                            Console.WriteLine("Game started");
                        }

                        var parsedPlayers = playerHandler.Handle(tcpPacket.Bytes);
                        players.AddRange(parsedPlayers);
                    }
                }
                if (stopwatch.Elapsed >= TimeSpan.FromSeconds(5)) break;
            }
            SetMyPlayerId(players);
            await SetStatsForAllPlayers(players);
            return players;
        }

        private static async Task SetStatsForAllPlayers(List<Player> players)
        {
            var playerStatsRetriever = new PlayerStatsRetriever(new DocumentProfileLoader());

            await Task.WhenAll(
                players.Select(async player =>
                {
                    var playerStats = await playerStatsRetriever.Retrieve(player.Name);
                    player.PlayerStats = playerStats;
                }));
        }

        private static void SetMyPlayerId(List<Player> players)
        {
            foreach (var player in players)
            {
                if (player.IsMe)
                {
                    var playerIds = Enumerable.Range(1, players.Count).Except(players.Select(x => x.Id)).ToList();
                    if (playerIds.Count > 1)
                    {
                        Console.WriteLine($"The following ids were possible for my player: {string.Join(",", playerIds)}");
                    }

                    player.Id = playerIds.FirstOrDefault();
                }
            }
        }
    }
}