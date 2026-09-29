using ApilntegratorTests.Interfaces;
using ApilntegratorTests.Json.Appsettings;
using ApilntegratorTests.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
        var apiSetting = Appsetting.FromConfig(context.Configuration).ApiSettings ?? new ApiSetting();
        services.AddSingleton(apiSetting);

        services.AddLogging(builder => builder.AddConsole());

        services.AddTransient<LoggingHandler>();
        services.AddSingleton<AuthorizationHandler>();

        services.AddRefitClient<IAuthApi>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri(apiSetting.BaseUrl))
            .AddHttpMessageHandler<LoggingHandler>();

        services.AddRefitClient<IAutomationApi>()
            .ConfigureHttpClient(client => client.BaseAddress = new Uri(apiSetting.BaseUrl))
            .AddHttpMessageHandler<LoggingHandler>()
            .AddHttpMessageHandler<AuthorizationHandler>();

        services.AddSingleton<IAutomationTaskApi, AutomationTaskApi>();
    }
}
