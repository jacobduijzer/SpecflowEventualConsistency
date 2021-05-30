using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SpecflowEventualConsistency.Application;
using SpecflowEventualConsistency.Domain;
using SpecflowEventualConsistency.Infrastructure;

namespace SpecflowEventualConsistency.WebApi
{
    public class Startup
    {
        private IConfiguration _configuration;
        
        public Startup(IConfiguration configuration) =>
            _configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .Configure<RabbitMqSettings>(_configuration.GetSection(nameof(RabbitMqSettings)))
                .AddDbContext<AppDbContext>(options => options.UseNpgsql(_configuration.GetConnectionString("SpecflowEventual")))
                .AddMediatR(cfg => cfg.AsScoped(), typeof(NewOrdersCommand).GetTypeInfo().Assembly)
                .AddScoped(typeof(IRepository<>), typeof(Repository<>))
                .AddScoped<EventPublisher>()
                .AddControllers();
            
            services.AddSwaggerGen(c =>
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "SpecflowEventualConsistency.WebApi", Version = "v1"}));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext appDbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SpecflowEventualConsistency.WebApi v1"));

            // app.UseHttpsRedirection();

            appDbContext.Database.EnsureDeleted();
            appDbContext.Database.EnsureCreated();

            app
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}