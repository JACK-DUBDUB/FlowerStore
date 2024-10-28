namespace Assessment2_MVC_API.Settings
{
    public class MongoDbConfig
    {

        public string Name { get; init; } = string.Empty;
        public string Host { get; init; } = string.Empty;
        public int Port { get; init; }
        public string ConnectionString => $"mongodb://{Host}:{Port}";
    }
}
