using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(BackgroundJobFunction.Startup))]
namespace BackgroundJobFunction
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient("PublicApi", client =>
            {
                client.BaseAddress = new Uri("https://api.publicapis.org/");
            });
        }
    }
}
