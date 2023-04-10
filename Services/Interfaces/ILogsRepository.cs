using Domain;

namespace Services.Interfaces
{
    public interface ILogsRepository
    {
        public Task<List<Log>> GetLogsWithinRange(DateTime startDate, DateTime endDateTime);
        public Task SaveLog(ExecutionStatus executionStatus);
        public Task<bool?> IsSuccessfulLog(DateTime date);
    }
}
