using System.Data;
using AdoNetCore.AseClient;

namespace SolucionDA.DatabaseAccess
{
    public class SybaseConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, string> _connectionStrings;

        public SybaseConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionStrings = new Dictionary<string, string>();

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
            return new AseConnection(connStr);
        }
    }
}
