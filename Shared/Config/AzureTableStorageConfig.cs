namespace Shared.Config
{
    public class AzureTableStorageConfig
    {
        public string Endpoint { get; set; }
        public string LogsTableName { get; set; }
        public string StorageAccountKey { get; set; }
        public string StorageAccountName { get; set; }
    }
}
