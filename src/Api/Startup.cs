using System.Threading;
using Contracts;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Api
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(options =>
            {
                options.UseSqlServerStorage(
                    "Server=localhost; Database=HangfireSample; User Id=sa;Password=StupidPassw0rd;");
            });
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

                endpoints.MapPost(
                    "/queue/{jobId}",
                    async context =>
                    {
                        var jobId = (string) context.Request.RouteValues["jobId"];
                        var enqueuedId = BackgroundJob
                            .Enqueue<IHandleJob>(
                                handler => handler.HandleAsync(jobId, CancellationToken.None));
                    });
            });
        }
    }
}