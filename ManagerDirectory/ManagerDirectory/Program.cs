﻿#region usings
using System;
using System.Threading.Tasks;
using ManagerDirectory.ConsoleView;
using ManagerDirectory.Infrastructure.Models;
using ManagerDirectory.Infrastructure.Repositories;
using ManagerDirectory.Services;
using ManagerDirectory.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
#endregion

namespace ManagerDirectory
{
	class Program
	{
		static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            Console.Title = "PersonalTerminal";

            if (host.Services.GetService(typeof(ManagerService)) is ManagerService manager)
            {
                await manager.RunAsync();
                await manager.StartAsync();
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureServices);

        static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddSingleton<ManagerService>();
            services.AddTransient<Receiver>();
            services.AddTransient<Displaying>();
            services.AddTransient<Repository>();
            services.AddTransient<CustomValidation>();
            services.AddTransient<InformingService>();
            services.AddTransient<CurrentPath>();
        }
    }
}
