using FileStore.Application.Interfaces;

namespace FileStore.Persistence.Dapper
{
    public class Context : IContext
    {
        private readonly string _connectionString;

        public Context(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}
