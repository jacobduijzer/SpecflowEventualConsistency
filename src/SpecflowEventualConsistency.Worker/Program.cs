using System;
using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpecflowEventualConsistency.Application;
using SpecflowEventualConsistency.Domain;
using SpecflowEventualConsistency.Infrastructure;

namespace SpecflowEventualConsistency.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    services
                        .Configure<RabbitMqSettings>(configuration.GetSection(nameof(RabbitMqSettings)))
                        .AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("SpecflowEventual")))
                        .AddMediatR(cfg => cfg.AsScoped(), typeof(NewOrdersCommand).GetTypeInfo().Assembly)
                        .AddScoped(typeof(IRepository<>), typeof(Repository<>));
                    
                    services.AddHostedService<Worker>();
                });
    }
}