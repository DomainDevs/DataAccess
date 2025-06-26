using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;


namespace SolucionDA.Repositories;

public class SqlServerRepository<T> : IRepository<T> where T : class
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction _transaction;

    public SqlServerRepository(IDbConnection connection, IDbTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _connection.QueryAsync<T>($"SELECT * FROM {typeof(T).Name}", transaction: _transaction);
    }

    public async Task<T> GetByIdAsync(object key)
    {
        string sql;
        DynamicParameters parameters = new();

        if (key is IDictionary<string, object> keys)
        {
            var conditions = string.Join(" AND ", keys.Select(k =>
            {
                parameters.Add(k.Key, k.Value);
                return $"{k.Key} = @{k.Key}";
            }));
            sql = $"SELECT * FROM {typeof(T).Name} WHERE {conditions}";
        }
        else
        {
            parameters.Add("Id", key);
            sql = $"SELECT * FROM {typeof(T).Name} WHERE Id = @Id";
        }

        return (await _connection.QueryAsync<T>(sql, parameters, _transaction)).FirstOrDefault();
    }

    public async Task<int> InsertAsync(T entity)
    {
        var type = typeof(T);
        var tableName = type.Name;

        var properties = type.GetProperties()
            .Where(p => p.CanRead && !string.Equals(p.Name, "Id", System.StringComparison.OrdinalIgnoreCase))
            .ToList();

        var columnNames = string.Join(", ", properties.Select(p => p.Name));
        var paramNames = string.Join(", ", properties.Select(p => "@" + p.Name));

        var sql = $"INSERT INTO {tableName} ({columnNames}) VALUES ({paramNames})";
        return await _connection.ExecuteAsync(sql, entity, _transaction);
    }

    public async Task<int> UpdateAsync(T entity)
    {
        var type = typeof(T);
        var tableName = type.Name;
        var properties = type.GetProperties().Where(p => p.CanRead).ToList();

        var keyProps = properties.Where(p => p.Name.EndsWith("Id") || p.Name.ToLower() == "id").ToList();
        var nonKeyProps = properties.Except(keyProps).ToList();

        var setClause = string.Join(", ", nonKeyProps.Select(p => $"{p.Name} = @{p.Name}"));
        var whereClause = string.Join(" AND ", keyProps.Select(p => $"{p.Name} = @{p.Name}"));

        var sql = $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";
        return await _connection.ExecuteAsync(sql, entity, _transaction);
    }

    public async Task<int> DeleteAsync(object key)
    {
        string sql;
        DynamicParameters parameters = new();

        if (key is IDictionary<string, object> keys)
        {
            var conditions = string.Join(" AND ", keys.Select(k =>
            {
                parameters.Add(k.Key, k.Value);
                return $"{k.Key} = @{k.Key}";
            }));
            sql = $"DELETE FROM {typeof(T).Name} WHERE {conditions}";
        }
        else
        {
            parameters.Add("Id", key);
            sql = $"DELETE FROM {typeof(T).Name} WHERE Id = @Id";
        }

        return await _connection.ExecuteAsync(sql, parameters, _transaction);
    }

    public async Task<IEnumerable<T>> ExecuteStoredProcedureAsync(string storedProcedure, object parameters)
    {
        return await _connection.QueryAsync<T>(
            storedProcedure,
            parameters,
            _transaction,
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<IEnumerable<TResult>> ExecuteStoredProcedureAsync<TResult>(string storedProcedure, object parameters)
    {
        return await _connection.QueryAsync<TResult>(
            storedProcedure,
            parameters,
            _transaction,
            commandType: CommandType.StoredProcedure
        );
    }

}