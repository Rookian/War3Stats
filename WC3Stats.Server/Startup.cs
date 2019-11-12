using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;

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
            services.AddSingleton(new BackgroundServiceConfiguration(Configuration.GetValue<bool>("Simulate")));

            services.AddSignalR().AddNewtonsoftJsonProtocol(options =>
            {

                options.PayloadSerializerSettings.Converters.Add(new StringEnumConverter());
            });
            services.AddHostedService<War3BackgroundService>();
            services.AddSpaStaticFiles(x => x.RootPath = "wwwroot");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(x => x.MapHub<Wc3Hub>("/wc3"));
            app.UseSpa(builder => { if(env.IsDevelopment()) builder.UseProxyToSpaDevelopmentServer("http://localhost:4200"); });
            app.UseSpaStaticFiles();
        }
    }
}
