using System.Collections.Generic;
using System.Linq;

namespace WC3Stats
{
    public class MyPlayerHandler : IPlayerHandler<Player>
    {
        private static readonly byte[] Pattern = { 0x00, 0x00, 0xf7, 0x1e };
        public bool Accepts(byte[] bytes)
        {
            var indexes = GetMarkerIndexes(bytes);
            return indexes.Any();
        }

        public Player Handle(byte[] bytes)
        {
            var index = GetMarkerIndexes(bytes).First();
            return Player.ParsePlayer(bytes, index + 21, true);
        }

        private static IEnumerable<int> GetMarkerIndexes(byte[] bytes)
        {
            return bytes.GetIndexes(Pattern).ToArray();
        }
    }
}