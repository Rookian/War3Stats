using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            services.AddCors(x => x.AddPolicy("CorsPolicy",
                builder => builder
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()));
            services.AddSignalR();
            services.AddHostedService<War3BackgroundService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");
            app.UseRouting();

            app.UseEndpoints(x => x.MapHub<Wc3Hub>("/wc3"));
        }
    }
}
