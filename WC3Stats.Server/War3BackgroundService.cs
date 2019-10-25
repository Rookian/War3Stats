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
            var x = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                x++;
                await _hub.Clients.All.Send(x.ToString());
                await Task.Delay(1000);
            }
        }
    }
}