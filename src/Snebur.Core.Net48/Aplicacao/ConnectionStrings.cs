#if NET6_0_OR_GREATER
namespace System.Configuration
{
    public class ConnectionStringSettings
    {
        public string Name { get; set; }

        public string ConnectionString { get; set; }

        public string ProviderName { get; set; }
    }
}
#endif
