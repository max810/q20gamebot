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
            services.Configure<BotConfig>(Configuration.GetSection("Bot"));
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("CONNECTION STRING \n");
            Console.WriteLine(dbUrl);
            Console.WriteLine("\n");
            var R = new Regex(@"[a-z]+:\/\/[a-z]+:[a-z0-9]+@[a-z0-9\.-]+:[0-9]+\/[a-z0-9]+");
            var res = R.Split(dbUrl);
            Console.WriteLine("dbtype: {0}", res[0]);
            Console.WriteLine("username: {0}", res[1]);
            Console.WriteLine("pwd: {0}", res[2]);
            Console.WriteLine("host: {0}", res[3]);
            Console.WriteLine("port: {0}", res[4]);
            Console.WriteLine("dname: {0}", res[5]);
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Environment.NewLine);
            services.AddHangfire(x => x.UsePostgreSqlStorage(Environment.GetEnvironmentVariable("DATABASE_URL")));
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
