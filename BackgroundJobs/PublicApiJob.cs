using Domain;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Services;
using Services.Interfaces;

namespace BackgroundJobs
{
    public class PublicApiJob
    {
        private readonly ILogger _logger;
        private readonly ILogsRepository _logsRepository;
        private readonly IPayloadRepository _payloadRepository;
        private readonly IPublicAPIService _publicApiService;

        public PublicApiJob(ILoggerFactory loggerFactory, ILogsRepository logsRepository,
            IPayloadRepository payloadRepository, IPublicAPIService publicApiService)
        {
            _logsRepository = logsRepository;
            _payloadRepository = payloadRepository;
            _publicApiService = publicApiService;
            _logger = loggerFactory.CreateLogger<PublicApiJob>();
        }

        [Function("PublicApiJob")]
        public async Task Run([TimerTrigger("0 */1 * * * *", RunOnStartup = true)] MyInfo myTimer)
        {
            try
            {
                _logger.LogInformation($"PublicApiJob started at: {DateTime.UtcNow}");
                var response = await _publicApiService.GetData();
                if (response.IsSuccess)
                {

                    var logEntry = LogEntity.CreateSuccessLogEntity();
                    await _logsRepository.SaveLog(ExecutionStatus.Success);
                    _logger.LogInformation($"API request succeeded at: {DateTime.UtcNow}");
                    await _payloadRepository.SavePayload(logEntry.Date, response.Payload);
                    _logger.LogInformation(response.Payload);
                }
                else
                {
                    await _logsRepository.SaveLog(ExecutionStatus.Failure);
                    _logger.LogInformation($"API request failed at: {DateTime.UtcNow}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }

        }

    }



    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
