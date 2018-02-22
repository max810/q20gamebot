using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Bot.Models;
using Hangfire;
using Hangfire.AspNetCore;
using Hangfire.PostgreSql.Annotations;
using Hangfire.PostgreSql;

namespace Bot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //var builder = new ConfigurationBuilder()
            // .SetBasePath(Directory.GetCurrentDirectory())
            // .AddJsonFile("appsettings.json");

            //Configuration = builder.Build();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<BotConfig>(Configuration.GetSection("Bot"));
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Environment.GetEnvironmentVariable("DATABASE_URL"));
            Console.WriteLine(Environment.NewLine);
            services.AddHangfire(x => x.UsePostgreSqlStorage(Configuration.GetConnectionString(Environment.GetEnvironmentVariable("DATABASE_URL"))));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHangfireServer();
            app.UseMvc();
        }
    }
}
