using ApilntegratorTests.Interfaces;
using ApilntegratorTests.Json.Appsettings;
using ApilntegratorTests.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;

namespace ApilntegratorTests;

public class Startup
{
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

    public void ConfigureServices(IServiceCollection services, HostBuilderContext context)
    {
        services.AddLogging(builder => builder.AddConsole());
        services.AddOptions<ApiSetting>().Bind(context.Configuration.GetSection(nameof(ApiSetting)));
        services.AddSingleton<AuthorizationHandler>();
        services.AddSingleton<LoggingHandler>();

        services.AddRefitClient<IAuthApi>()
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var baseUrl = serviceProvider.GetRequiredService<IOptions<ApiSetting>>().Value.BaseUrl;
                client.BaseAddress = new Uri(baseUrl);
            })
            .AddHttpMessageHandler<LoggingHandler>();

        services.AddRefitClient<IItegratorAutomationApi>()
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var baseUrl = serviceProvider.GetRequiredService<IOptions<ApiSetting>>().Value.BaseUrl;
                client.BaseAddress = new Uri(baseUrl);
            })
            .AddHttpMessageHandler<LoggingHandler>()
            .AddHttpMessageHandler<AuthorizationHandler>();
    }
}