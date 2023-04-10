using Azure;
using Azure.Data.Tables;
using Domain;
using Services.Extensions;

namespace Services
{
    public class LogEntity : Log, ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public LogEntity()
        {

        }

        private static DateTime GetDate()
        {
            return DateTime.UtcNow;
        }
        public static LogEntity CreateSuccessLogEntity()
        {
            var logDate = GetDate();
            return new LogEntity()
            {
                PartitionKey = logDate.GetPartitionKeyFormat(),
                RowKey = logDate.GetRowKeyFormat(),
                Date = logDate,
                ExecutionStatus = Domain.ExecutionStatus.Success
            };
        }

        public static LogEntity CreateFailureLogEntity()
        {
            var logDate = GetDate();
            return new LogEntity()
            {
                PartitionKey = logDate.GetPartitionKeyFormat(),
                RowKey = logDate.GetRowKeyFormat(),
                Date = logDate,
                ExecutionStatus = Domain.ExecutionStatus.Failure
            };
        }
    }
}
