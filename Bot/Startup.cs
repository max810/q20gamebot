using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Bot.Models.BotConfigs;
using Microsoft.EntityFrameworkCore;
using Bot.VpnBot.Models;
using Bot.Q20GameBot.Models;
using Bot.Models.VpnBot.Schedulers;

namespace Bot
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
            services.Configure<VpnBotConfig>(Configuration.GetSection("Bots:VpnBot"));
            services.Configure<Q20GameBotConfig>(Configuration.GetSection("Bots:Q20GameBot"));
            services.AddSingleton<PasswordUpdateProvider>();
            services.AddSingleton<PasswordUpdateHostedService>();
            services.AddMvc();

            var connectionString = @"Server=tcp:bots-server.database.windows.net,1433;Initial Catalog=Q20GameSessions;Persist Security Info=False;User ID=max810;Password=Kis2012thebest;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            services.AddDbContext<SessionsContext>(options => options.UseSqlServer(connectionString));
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc(x => x.MapRoute("default", 
                "{controller=vpnbot}/{action=start}"));
        }
    }
}
