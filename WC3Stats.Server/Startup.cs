using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WC3Stats.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddSignalR();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(x => x.WithOrigins("http://127.0.0.1:5500").AllowAnyHeader().WithMethods("GET", "POST").AllowCredentials());
            app.UseRouting();

            app.UseEndpoints(x => x.MapHub<WC3Hub>("/wc3"));
        }
    }

    public class WC3Hub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync($"{Context.ConnectionId} connected.");
        }

        public async Task Send(string message)
        {
            await Clients.All.SendAsync("Send", message);
        }
    }

}
