using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PacketDotNet;
using SharpPcap;

namespace WC3Stats
{
    public class PlayerStream : IObservable<ImmutableList<Player>>
    {
        private readonly List<IObserver<ImmutableList<Player>>> _observers = new List<IObserver<ImmutableList<Player>>>();
        public IDisposable Subscribe(IObserver<ImmutableList<Player>> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            return new Unsubscriber(_observers, observer);
        }

        public async Task Start()
        {
            var device = IPAddress.Parse("5.42.181.0").GetDefaultCaptureDevice();
            device.Open(DeviceMode.Promiscuous, 1000);
            device.Filter = "ip and tcp";

            var handlers = new List<IPlayerHandler> { new OpponentHandler(), new MyPlayerHandler() };
            var playerStatsRetriever = new PlayerStatsRetriever(new DocumentProfileLoader());

            var stopwatch = new Stopwatch();
            do
            {
                var rawCapture = device.GetNextPacket();
                if (rawCapture == null) continue;

                var packet = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);
                if (!(packet.PayloadPacket.PayloadPacket is TcpPacket tcpPacket)) continue;
                if (!tcpPacket.IsInIpRange()) continue;

                foreach (var handler in handlers)
                {

                    if (handler.Accepts(tcpPacket.Bytes))
                    {
                        stopwatch.Start();
                        var players = handler.Handle(tcpPacket.Bytes).ToImmutableList();
                        await Task.WhenAll(players.Select(async player =>
                        {
                            var playerStats = await playerStatsRetriever.Retrieve(player.Name);
                            player.PlayerStats = playerStats;
                        }));

                        foreach (var observer in _observers)
                        {
                            observer.OnNext(players);
                        }
                    }
                }

                if (stopwatch.Elapsed >= TimeSpan.FromSeconds(5))
                {
                    foreach (var observer in _observers)
                    {
                        observer.OnCompleted();
                    }
                    return;
                }

            } while (true);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<ImmutableList<Player>>> _observers;
            private readonly IObserver<ImmutableList<Player>> _observer;

            public Unsubscriber(List<IObserver<ImmutableList<Player>>> observers, IObserver<ImmutableList<Player>> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }

    public class PlayerObserver : IObserver<ImmutableList<Player>>
    {
        private ImmutableList<Player> _players = ImmutableList.Create<Player>();
        public void OnCompleted()
        {
            foreach (var player in _players)
            {
                if (player.IsMe)
                {
                    var playerIds = Enumerable.Range(1, _players.Count + 1).Except(_players.Select(x => x.Id)).ToList();
                    if (playerIds.Count > 1)
                    {
                        Console.WriteLine($"The following ids were possible for my player: {string.Join(",", playerIds)}");
                    }
                    player.Id = playerIds.FirstOrDefault();
                }
            }

            foreach (var teamGroup in _players.GroupBy(x => x.Team))
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

        public void OnError(Exception error)
        {

        }

        public void OnNext(ImmutableList<Player> players)
        {
            foreach (var player in players)
            {
                _players = _players.Add(player);
                var playerStats = player.PlayerStats;
                Console.WriteLine(
                    $"{player.Id} - {player.Name.PadRight(14)} {player.Race.ToString().PadRight(9)} - Solo: {playerStats.SoloWinRate.ToString(CultureInfo.InvariantCulture).PadRight(4)}% - Team: {playerStats.TeamWinRate.ToString(CultureInfo.InvariantCulture).PadRight(4)}%");
            }
        }
    }
}