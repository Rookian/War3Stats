using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Shouldly;
using Xunit;

namespace WC3Stats.Tests
{
    public class Tests
    {
        [Fact]
        public void Should_parse_one_opponent()
        {
            // Arrange
            var bytes = new byte[]
            {
                0x88, 0xd7, 0xf6, 0x40, 0x41, 0xd9, 0xf0, 0x7d,
                0x68, 0x5b, 0x36, 0x0a, 0x08, 0x00, 0x45, 0x00,
                0x00, 0x78, 0x42, 0xb5, 0x40, 0x00, 0x74, 0x06,
                0x48, 0x8c, 0x05, 0x2a, 0xb5, 0x6a, 0xc0, 0xa8,
                0x00, 0x02, 0x17, 0xe0, 0xf2, 0x53, 0x03, 0xcc,
                0xd5, 0xcb, 0xc9, 0x89, 0x2f, 0x34, 0x50, 0x18,
                0xfa, 0xf0, 0x53, 0xa1, 0x00, 0x00, 0xf7, 0x06,
                0x39, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x73,
                0x74, 0x65, 0x70, 0x33, 0x33, 0x00, 0x08, 0x52,
                0x41, 0x6b, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xf7,
                0x47, 0x0f, 0x00, 0x02, 0x01, 0x03, 0x01, 0x00,
                0x00, 0x02, 0x03, 0x11, 0x32, 0x18, 0xf7, 0x0a,
                0x04, 0x00, 0xf7, 0x0b, 0x04, 0x00
            };

            var opponentHandler = new OpponentHandler();

            // Act
            var accepts = opponentHandler.Accepts(bytes);
            var opponents = opponentHandler.Handle(bytes).ToList();

            // Assert

            accepts.ShouldBeTrue();
            opponents.Count.ShouldBe(1);
            opponents[0].Name.ShouldBe("step33");
            opponents[0].Race.ShouldBe(Races.NightElf);
        }

        [Fact]
        public async Task Should_get_stats_from_player()
        {
            // Arrange
            var playerStats = new PlayerStatsRetriever(new FakeLoader());

            // Act
            var stats = await playerStats.Retrieve("Rookianx");

            // Assert
            stats.SoloWins.ShouldBe(4);
            stats.SoloLosses.ShouldBe(1);
            stats.SoloGames.ShouldBe(5);
            stats.SoloWinRate.ShouldBe(80);
            stats.SoloLevel.ShouldBe(4);

            stats.TeamWins.ShouldBe(27);
            stats.TeamLosses.ShouldBe(16);
            stats.TeamGames.ShouldBe(43);
            stats.TeamWinRate.ShouldBe(62.8m);
            stats.TeamLevel.ShouldBe(12);

            stats.WinRate.ShouldBe(64.6m);
        }

        [Fact]
        public void Should_parse_multiple_opponents()
        {
            // Arrange
            var bytes = File.ReadAllBytes("opp-multi.txt");
            var opponentHandler = new OpponentHandler();

            // Act
            var opponents = opponentHandler.Handle(bytes).ToList();

            // Assert
            opponents.Count.ShouldBe(7);
            opponents[0].Name.ShouldBe("coccode");
            opponents[0].Race.ShouldBe(Races.Undead);

            opponents[1].Name.ShouldBe("conky");
            opponents[1].Race.ShouldBe(Races.Random);

            opponents[2].Name.ShouldBe("Tyrie");
            opponents[2].Race.ShouldBe(Races.Undead);
        }
    }

    public class FakeLoader : IDocumentProfileLoader
    {
        public Task<IDocument> Load(string name)
        {
            var fileName = $"{name}.html";

            if (!File.Exists(fileName))
                throw new ArgumentException($"The file {Path.GetFullPath(fileName)} does not exist.");

            var config = Configuration.Default.WithDefaultLoader();
            var browsingContext = BrowsingContext.New(config);

            var document = browsingContext.OpenAsync(x =>
            {
                var readAllText = File.ReadAllText(fileName);
                x.Content(readAllText);
            });

            return document;
        }
    }
}