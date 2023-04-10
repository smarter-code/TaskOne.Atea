namespace Services.Interfaces
{
    public interface IPayloadRepository
    {
        public Task SavePayload(DateTime dateTime, string result);
        public Task<object> GetPayload(DateTime dateTime);
    }
}
