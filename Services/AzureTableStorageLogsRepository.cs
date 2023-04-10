using Azure.Data.Tables;
using Domain;
using Microsoft.Extensions.Options;
using Services.Extensions;
using Services.Interfaces;
using Shared.Config;

namespace Services
{
    public class AzureTableStorageLogsRepository : ILogsRepository
    {
        private readonly TableClient _tableClient;

        public AzureTableStorageLogsRepository(IOptions<AzureTableStorageConfig> azureTableStorageConfigOptions)
        {
            var azureTableStorageConfig = azureTableStorageConfigOptions.Value;
            _tableClient = new TableClient(
                new Uri(azureTableStorageConfig.Endpoint),
                azureTableStorageConfig.LogsTableName,
                new TableSharedKeyCredential(azureTableStorageConfig.StorageAccountName, azureTableStorageConfig.StorageAccountKey));
        }
        public async Task<List<Log>> GetLogsWithinRange(DateTime startDate, DateTime endDateTime)
        {
            List<Log> logs = new List<Log>();
            string startPartitionKey = startDate.GetPartitionKeyFormat();
            string endPartitionKey = endDateTime.GetPartitionKeyFormat();
            string startRowKey = startDate.GetRowKeyFormat();
            string endRowKey = endDateTime.GetRowKeyFormat();
            string filter = $"PartitionKey ge '{startPartitionKey}' and PartitionKey le '{endPartitionKey}' and RowKey ge '{startRowKey}' and RowKey le '{endRowKey}'";

            await foreach (var log in _tableClient.QueryAsync<LogEntity>(filter))
            {

                logs.Add(Log.CreateLog(log.Date, log.ExecutionStatus));
            }

            return logs;
        }

        public async Task SaveLog(ExecutionStatus executionStatus)
        {
            LogEntity logEntity;

            if (executionStatus == ExecutionStatus.Success)
            {
                logEntity = LogEntity.CreateSuccessLogEntity();
            }
            else
            {
                logEntity = LogEntity.CreateFailureLogEntity();
            }

            _tableClient.AddEntity(logEntity);
        }

        public async Task<bool?> IsSuccessfulLog(DateTime date)
        {
            var log = await GetSpecificLogPayload(date);
            //The user requested non existing log
            if (log == null)
                return null;
            return log.ExecutionStatus == ExecutionStatus.Success;
        }

        private async Task<Log> GetSpecificLogPayload(DateTime date)
        {
            List<Log> logs = new List<Log>();
            string partitionKey = date.GetPartitionKeyFormat();
            string rowKey = date.GetRowKeyFormat();
            string filter = $"PartitionKey eq '{partitionKey}' and RowKey eq '{rowKey}'";
            await foreach (var log in _tableClient.QueryAsync<LogEntity>(filter))
            {
                logs.Add(Log.CreateLog(log.Date, log.ExecutionStatus));
            }
            if (logs.Count > 1)
                throw new ArgumentOutOfRangeException(
                    "Something went wrong, a specific minute should only have one log");

            return logs.FirstOrDefault();
        }
    }
}
