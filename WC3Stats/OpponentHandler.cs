using System.Collections.Generic;
using System.Linq;

namespace WC3Stats
{
    public class OpponentHandler : IPlayerHandler
    {
        private static readonly byte[] Pattern = { 0x00, 0x00, 0xf7, 0x06 };

        public bool Accepts(byte[] bytes)
        {
            var indexes = GetMarkerIndexes(bytes);
            return indexes.Any();
        }

        public IEnumerable<Player> Handle(byte[] bytes)
        {
            var indexes = GetMarkerIndexes(bytes);

            foreach (var index in indexes)
            {
                var (isValid, player) = Player.TryParsePlayer(bytes, index + 11, false);

                if (isValid)
                    yield return player;
            }
        }

        private static IEnumerable<int> GetMarkerIndexes(byte[] bytes)
        {
            return bytes.GetIndexes(Pattern).ToArray();
        }
    }
}