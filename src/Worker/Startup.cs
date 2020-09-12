using System;
using Contracts;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Worker
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire((s, options) =>
            {
                options.UseSqlServerStorage(
                    _configuration.GetValue<string>("ConnectionString"),
                    new SqlServerStorageOptions {QueuePollInterval = TimeSpan.FromSeconds(1)});
            });

            services.AddHangfireServer(options =>
            {
                options.ServerName = "SampleWorker";
                //options.WorkerCount = 1;
            });

            services.AddScoped<IHandleJob, HandleJob>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHangfireDashboard("/dashboard");

                endpoints.MapGet(
                    "/",
                    async context => { await context.Response.WriteAsync("Hello World!"); });
            });
        }
    }
}