namespace Login.models.setting
{
    public interface IDatabase
    {
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }

    }
}
