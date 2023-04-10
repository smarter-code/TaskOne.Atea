using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services;
using Services.Interfaces;
using Shared.Config;


var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", true, false)
        .AddEnvironmentVariables()
    .Build();

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHttpClient("PublicApiHttpClient", client =>
        {
            client.BaseAddress = new Uri(configuration["PublicApiConfig:BaseUrl"]!);
        });
        services.AddScoped<ILogsRepository, AzureTableStorageLogsRepository>();
        services.AddScoped<IPayloadRepository, AzureBlobStoragePayloadRepository>();
        services.AddScoped<IPublicAPIService, PublicApiHttpClientService>();
        services.Configure<AzureBlobStorageConfig>(configuration.GetSection("AzureBlobStorageConfig"));
        services.Configure<AzureTableStorageConfig>(configuration.GetSection("AzureTableStorageConfig"));
        services.Configure<PublicApiConfig>(configuration.GetSection("PublicApiConfig"));
    })
    .Build();

host.Run();
