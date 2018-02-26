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
using System.Text.RegularExpressions;
using Npgsql;

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
            var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            //dbUrl = @"postgres://xljhfbybkapuyn:c77a6062449fcc40f6673e3cd1ae07300e99b06af39c97ae0920f7e98903ef8d@ec2-184-73-202-79.compute-1.amazonaws.com:5432/df11kgfp9edh3e".Trim();
            services.Configure<BotConfig>(Configuration.GetSection("Bot"));
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("CONNECTION STRING \n");
            Console.WriteLine(dbUrl);
            Console.WriteLine("\n");
            var urlSplitter = new Regex(@"[:@(\/)]");
            //var R = new Regex(@"[a-z]+:\/\/[a-z]+:[a-z0-9]+@[a-z0-9\.-]+:[0-9]+\/[a-z0-9]+");
            var res = urlSplitter.Split(dbUrl).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            // ENABLE SSL
            var connectionString = string.Format("Host={3};Port={4};Database={5};Username={1};Password={2};Pooling=true;Use SSL Stream=True;SSL Mode=Require;TrustServerCertificate=True",
                res);
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Environment.NewLine);
            //services.AddHangfire(x => x.UsePostgreSqlStorage(Environment.GetEnvironmentVariable("DATABASE_URL")));

            services.AddHangfire(x => x.UsePostgreSqlStorage(connectionString));
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
