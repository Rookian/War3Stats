using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace WC3Stats.Server
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Wc3Hub : Hub<IWc3>
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.Send("Connected Yeahaa!");
        }
    }

    public interface IWc3
    {
        Task Send(string message);
    }
}