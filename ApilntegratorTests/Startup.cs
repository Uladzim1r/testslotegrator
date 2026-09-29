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

        services.AddSingleton<AuthorizationHandler>();
        services.AddSingleton<LoggingHandler>();
        services.AddSingleton<IAutomationApi>(provider =>
        {
            var httpClientHandler = new HttpClientHandler();

            var loggingHandler = provider.GetRequiredService<LoggingHandler>();
            loggingHandler.InnerHandler = httpClientHandler;

            var authorizationHandler = provider.GetRequiredService<AuthorizationHandler>();
            authorizationHandler.InnerHandler = loggingHandler;

            var client = new HttpClient(authorizationHandler)
            {
                BaseAddress = new Uri(apiSetting.BaseUrl)
            };

            return RestService.For<IAutomationApi>(client);
        });

        services.AddSingleton<IAutomationTaskApi, AutomationTaskApi>();
    }
}
