using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PacketDotNet;
using SharpPcap;

namespace WC3Stats
{
    public class Game
    {
        public List<Player> Players { get; } = new List<Player>();

        public bool IsReady => Players.Any(x => x.IsMe) &&
                               Players.Count > 1;
    }

    public class GameMonitor
    {
        public static async Task<Game> Start()
        {
            var device = GetDefaultCaptureDevice(IPAddress.Parse("5.42.181.0"));
            device.Open(DeviceMode.Promiscuous, 1000);
            device.Filter = "ip and tcp";

            var game = new Game();
            while (!game.IsReady)
            {
                var rawCapture = device.GetNextPacket();
                if (rawCapture == null) continue;

                var packet = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                if (!(packet.PayloadPacket.PayloadPacket is TcpPacket tcpPacket)) continue;
                if (!IsInIpRange(tcpPacket)) continue;

                var opponentHandler = new OpponentHandler();
                if (opponentHandler.Accepts(tcpPacket.Bytes))
                {
                    Console.WriteLine("Opponent bytes");
                    game.Players.AddRange(opponentHandler.Handle(tcpPacket.Bytes));
                }

                var myPlayerHandler = new MyPlayerHandler();
                if (myPlayerHandler.Accepts(tcpPacket.Bytes))
                {
                    Console.WriteLine("My bytes");
                    game.Players.Add(myPlayerHandler.Handle(tcpPacket.Bytes));
                }

                foreach (var player in game.Players)
                {
                    if (player.IsMe)
                    {
                        player.Id = Enumerable.Range(1, 8).Except(game.Players.Select(x => x.Id)).FirstOrDefault();
                    }
                }
            }

            var playerStatsRetriever = new PlayerStatsRetriever(new DocumentProfileLoader());

            foreach (var player in game.Players)
            {
                var playerStats = await playerStatsRetriever.Retrieve(player.Name);
                player.PlayerStats = playerStats;
            }

            return game;
        }

        private static bool IsInIpRange(Packet tcpPacket)
        {
            var ipPacket = (IPPacket)tcpPacket.ParentPacket;
            var source = ipPacket.SourceAddress.ToString();
            var destination = ipPacket.DestinationAddress.ToString();

            return destination.StartsWith("5.42.181") || source.StartsWith("5.42.181");
        }


        private static ICaptureDevice GetDefaultCaptureDevice(IPAddress ip)
        {
            var nicName = NIC.GetDefaultId(ip);
            var device = CaptureDeviceList.Instance
                .SingleOrDefault(x => x.Name.Replace("rpcap://\\Device\\NPF_", "") == nicName);
            return device;
        }
    }
}