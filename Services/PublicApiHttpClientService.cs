using Microsoft.Extensions.Options;
using Services.Interfaces;
using Shared.Config;

namespace Services
{
    public class PublicApiHttpClientService : IPublicAPIService
    {
        private readonly HttpClient _client;
        private readonly PublicApiConfig _publicApiConfig;
        public PublicApiHttpClientService(IHttpClientFactory httpClientFactory
            , IOptions<PublicApiConfig> publicApiConfigOptions)
        {
            _client = httpClientFactory.CreateClient("PublicApiHttpClient");
            _publicApiConfig = publicApiConfigOptions.Value;
        }

        public async Task<ApiResult> GetData()
        {
            try
            {
                var response = await _client.GetAsync(_publicApiConfig.RandomGetRoute);
                if (response.IsSuccessStatusCode)
                {
                    var payload = await response.Content.ReadAsStringAsync();
                    return new ApiResult()
                    {
                        IsSuccess = true,
                        Payload = payload
                    };
                }

                return new ApiResult()
                {
                    IsSuccess = false
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ApiResult()
                {
                    IsSuccess = false
                };
            }

        }
    }

    public class ApiResult
    {
        public bool IsSuccess { get; set; }
        public string Payload { get; set; }
    }
}
