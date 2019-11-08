using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WC3Stats.Server
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Wc3Hub : Hub<IWc3>
    {

    }

    public interface IWc3
    {
        Task Send(List<Player> players);
        Task GameFound();
    }
}