namespace Domain
{
    public class Log
    {
        public DateTime Date { get; set; }
        public ExecutionStatus ExecutionStatus { get; set; }

        public static Log CreateLog(DateTime date, ExecutionStatus executionStatus)
        {
            return new Log() { Date = date, ExecutionStatus = executionStatus };
        }

    }


    public enum ExecutionStatus
    {
        Success,
        Failure
    }
}
