using AdoNetCore.AseClient;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;

namespace SolucionDA.DatabaseAccess;

public class MultiDbConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _configuration;

    public MultiDbConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection(string dbAlias)
    {
        var connectionString = _configuration.GetSection("ConnectionStrings:" + dbAlias).Value;
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException($"No se encontró cadena de conexión para '{dbAlias}'.");

        return dbAlias.ToLowerInvariant() switch
        {
            "sqlserver" => new SqlConnection(connectionString),
            "sybase" => new AseConnection(connectionString),
            _ => throw new NotSupportedException($"Tipo de base de datos '{dbAlias}' no soportado.")
        };
    }
}