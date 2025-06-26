using System.Data;
using Microsoft.Extensions.Configuration;
using AdoNetCore.AseClient;     // Sap.Data.AseClient;

namespace SolucionDA.DatabaseAccess
{
    public class SybaseConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SybaseConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Sybase");
        }

        public IDbConnection CreateConnection()
            => new AseConnection(_connectionString);
    }
}
