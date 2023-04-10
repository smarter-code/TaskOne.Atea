namespace Services.Interfaces
{

    public interface IPublicAPIService
    {
        public Task<ApiResult> GetData();
    }
}
