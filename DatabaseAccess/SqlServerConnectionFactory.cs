using Microsoft.Data.SqlClient;
using System.Data;

namespace SolucionDA.DatabaseAccess
{
    public class SqlServerConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, string> _connectionStrings;

        public SqlServerConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionStrings = new Dictionary<string, string>();

            // Cargar todas las cadenas de conexión con sus alias desde appsettings.json
            var connectionSection = _configuration.GetSection("ConnectionStrings");
            foreach (var child in connectionSection.GetChildren())
            {
                _connectionStrings[child.Key] = child.Value!;
            }
        }

        public IDbConnection CreateConnection(string dbAlias)
        {
            if (!_connectionStrings.ContainsKey(dbAlias))
                throw new ArgumentException($"Alias de conexión no válido: '{dbAlias}'.");

            var connStr = _connectionStrings[dbAlias];
            return new SqlConnection(connStr);
        }
    }
}
