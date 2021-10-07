using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Natom.AccessMonitor.Extensions;
using Natom.AccessMonitor.Sync.Receiver.Entities.MQ;
using Natom.AccessMonitor.Sync.Receiver.Filters;

namespace Natom.AccessMonitor.Sync.Receiver
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
            services
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddHttpClient()
                .AddConfigurationService(refreshTimeMS: 30000)
                .AddCacheService()
                .AddAuthService(scope: "Sync.Receiver")
                .AddLoggerService(systemName: "Sync.Receiver", insertEachMS: 30000, bulkInsertSize: 10000)
                .AddMQProducerService(new ConfigurationMQ
                {
                    HostName = "",
                    Port = 0,
                    UserName = "",
                    Password = ""
                }, enableSsl: false);


            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(AuthorizationFilter));
                options.Filters.Add(typeof(ResultFilter));
            })
            .AddNewtonsoftJson();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Natom.AccessMonitor.Sync.Receiver", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Natom.AccessMonitor.Sync.Receiver v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
