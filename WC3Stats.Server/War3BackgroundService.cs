using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace WC3Stats.Server
{
    public class War3BackgroundService : BackgroundService
    {
        private readonly IHubContext<Wc3Hub, IWc3> _hub;

        public War3BackgroundService(IHubContext<Wc3Hub, IWc3> hub)
        {
            _hub = hub;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(100); // Need to delay else server is not starting (bug?!)
                var players = await GameMonitor.LookForPlayers();
                await _hub.Clients.All.Send(players);
                
            }
        }
    }
}