using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Text;

namespace WC3Stats
{
    public interface IDocumentProfileLoader
    {
        Task<IDocument> Load(string name);
    }

    public class DocumentProfileLoader : IDocumentProfileLoader
    {
        public async Task<IDocument> Load(string name)
        {
            var url = $"http://classic.battle.net/war3/ladder/W3XP-player-profile.aspx?Gateway=Northrend&PlayerName={name}";
            var config = Configuration.Default.WithDefaultLoader();
            var browsingContext = BrowsingContext.New(config);
            return await browsingContext.OpenAsync(new Url(url));
        }
    }
    public class PlayerStatsRetriever
    {
        private readonly IDocumentProfileLoader _documentProfileLoader;

        public PlayerStatsRetriever(IDocumentProfileLoader documentProfileLoader)
        {
            _documentProfileLoader = documentProfileLoader;
        }
        public async Task<PlayerStats> Retrieve(string name)
        {
            using (var doc = await _documentProfileLoader.Load(name))
            {
                var (soloWins, soloLosses, soloLevel) = GetStats("Solo Games", doc);
                var (teamWins, teamLosses, teamLevel) = GetStats("Team Games", doc);

                return new PlayerStats
                {
                    SoloLosses = soloLosses,
                    SoloWins = soloWins,
                    SoloLevel = soloLevel,
                    TeamLosses = teamLosses,
                    TeamWins = teamWins,
                    TeamLevel = teamLevel
                };
            }
        }

        private (int wins, int losses, int level) GetStats(string gameTypeLabel, IDocument doc)
        {
            var gameTypeElement = doc.All.SingleOrDefault(x => x.Text() == gameTypeLabel);
            if (gameTypeElement == null)
                return (0, 0, 0);

            var gameTypeTable = gameTypeElement
                .Ancestors<IHtmlTableElement>().First(x => x.Matches("table"));

            var gamesTable = gameTypeTable
                .GetChildrenRecursively().First(x => x.Text().StripLeadingTrailingSpaces() == "Wins:").GetAncestor<IHtmlTableElement>();

            var winElement = gamesTable.QuerySelector(("tbody > tr:nth-child(2) > td:nth-child(2) > b"));
            var lossElement = gamesTable.QuerySelector("tbody > tr:nth-child(3) > td:nth-child(2) > b");
            var levelElement = gameTypeTable.GetChildrenRecursively().First(x => x.Text().StripLeadingTrailingSpaces().StartsWith("Level"));

            int.TryParse(winElement.Text(), out var wins);
            int.TryParse(lossElement.Text(), out var losses);
            int.TryParse(levelElement.Text()
                    .Replace("\n", "").Replace("\t", "").Replace("Level ", ""), 
                    out var level);

            return (wins, losses, level);
        }
    }

    public class PlayerStats
    {
        public int SoloWins { get; set; }
        public int SoloLosses { get; set; }
        public int SoloGames => SoloWins + SoloLosses;
        public decimal SoloWinRate => WinRate(SoloWins, SoloGames);
        public int SoloLevel { get; set; }
        public int TeamWins { get; set; }
        public int TeamLosses { get; set; }
        public int TeamGames => TeamWins + TeamLosses;
        public decimal TeamWinRate => WinRate(TeamWins, TeamGames);
        public int TeamLevel { get; set; }

        private static decimal WinRate(int wins, int games) => games > 0 ?
            decimal.Round(100 * (decimal)wins / games, 1, MidpointRounding.AwayFromZero) :
            0;

        public override string ToString()
        {
            return $"{nameof(SoloWinRate)}: {SoloWinRate}, {nameof(TeamWinRate)}: {TeamWinRate}";
        }
    }

    public static class ElementExtensions
    {
        public static IEnumerable<IElement> GetChildrenRecursively(this IElement e)
        {
            return e.Children.Union(e.Children.SelectMany(c => c.GetChildrenRecursively()));
        }
    }
}