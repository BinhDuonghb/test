namespace Login.models.setting
{
    public class Database : IDatabase
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }
}
