using System.Collections.Generic;

namespace WC3Stats
{
    public interface IPlayerHandler
    {
        bool Accepts(byte[] bytes);
        IEnumerable<Player> Handle(byte[] bytes);
    }
}