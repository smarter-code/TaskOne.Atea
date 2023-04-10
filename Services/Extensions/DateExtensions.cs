namespace Services.Extensions
{
    public static class DateExtensions
    {
        public static string GetPartitionKeyFormat(this DateTime date)
        {
            return date.ToString("yyyyMM");
        }
        public static string GetRowKeyFormat(this DateTime date)
        {
            return date.ToString("yyyyMMddHHmm");
        }
    }
}
