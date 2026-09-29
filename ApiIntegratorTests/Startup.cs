using ApiIntegratorTests.Interfaces;
using ApiIntegratorTests.Json.Appsettings;
using ApiIntegratorTests.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;

namespace ApiIntegratorTests;

// ReSharper disable once UnusedType.Global
public class Startup
{
    // ReSharper disable once UnusedMember.Global
    public void ConfigureHost(IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureAppConfiguration(config =>
        {
            config.SetBasePath(AppContext.BaseDirectory);
            config.AddJsonFile("appsettings.json", optional: false);
            config.AddJsonFile("appsettings.local.json", optional: true);
            config.AddEnvironmentVariables();
        });
    }

    // ReSharper disable once UnusedMember.Global
    public void ConfigureServices(IServiceCollection services, HostBuilderContext context)
    {
        services.AddLogging(builder => builder.AddConsole());
        services.AddOptions<IntegratorApiConfig>().Bind(context.Configuration.GetSection(nameof(IntegratorApiConfig)));
        services.AddSingleton<AuthorizationHandler>();
        services.AddSingleton<LoggingHandler>();

        services.AddRefitClient<IAuthApi>()
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var baseUrl = serviceProvider.GetRequiredService<IOptions<IntegratorApiConfig>>().Value.BaseUrl;
                client.BaseAddress = new Uri(baseUrl);
            });
            //.AddHttpMessageHandler<LoggingHandler>();

        services.AddRefitClient<IIntegratorAutomationApi>()
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var baseUrl = serviceProvider.GetRequiredService<IOptions<IntegratorApiConfig>>().Value.BaseUrl;
                client.BaseAddress = new Uri(baseUrl);
            })
            //.AddHttpMessageHandler<LoggingHandler>()
            .AddHttpMessageHandler<AuthorizationHandler>();
    }
}
