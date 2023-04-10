using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BackgroundJobFunction
{
    public class PublicApiJob
    {
        private readonly HttpClient _client;
        public PublicApiJob(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("PublicApi");
        }
        [FunctionName("PublicApiJob")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"PublicApiJob started at: {DateTime.UtcNow}");
            var response = await _client.GetAsync("random?auth=null");
            if (response.IsSuccessStatusCode)
            {
                log.LogInformation($"API request succeeded at: {DateTime.UtcNow}");
                var result = await response.Content.ReadAsStringAsync();
                log.LogInformation(result);
            }
            else
            {
                log.LogInformation($"API request failed at: {DateTime.UtcNow}");
            }


        }
    }
}
