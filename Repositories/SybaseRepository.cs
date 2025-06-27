using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SolucionDA.Repositories
{
    public class SybaseRepository<T> : IRepository<T> where T : class
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private readonly string _tableName;

        public SybaseRepository(IDbConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
            _tableName = typeof(T).Name;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var sql = $"SELECT * FROM {_tableName}";
            return await _connection.QueryAsync<T>(sql, transaction: _transaction);
        }

        public async Task<T?> GetByIdAsync(Dictionary<string, object> keys)
        {
            var whereClause = string.Join(" AND ", keys.Keys.Select(k => $"{k} = @{k}"));
            var sql = $"SELECT * FROM {_tableName} WHERE {whereClause}";
            return await _connection.QueryFirstOrDefaultAsync<T>(sql, keys, _transaction);
        }

        public async Task<int> InsertAsync(T entity)
        {
            var props = typeof(T).GetProperties()
                .Where(p => p.CanRead && p.GetValue(entity) != null)
                .ToList();

            var columns = string.Join(", ", props.Select(p => p.Name));
            var values = string.Join(", ", props.Select(p => $"@{p.Name}"));
            var sql = $"INSERT INTO {_tableName} ({columns}) VALUES ({values})";

            return await _connection.ExecuteAsync(sql, entity, _transaction);
        }

        public async Task<int> UpdateAsync(T entity)
        {
            var props = typeof(T).GetProperties().Where(p => p.CanRead).ToList();
            var keys = props.Where(p => p.Name.EndsWith("Id") || p.Name.StartsWith("Tipo") || p.Name.StartsWith("Codigo")).ToList();

            var setters = string.Join(", ", props.Where(p => !keys.Contains(p)).Select(p => $"{p.Name} = @{p.Name}"));
            var where = string.Join(" AND ", keys.Select(p => $"{p.Name} = @{p.Name}"));

            var sql = $"UPDATE {_tableName} SET {setters} WHERE {where}";

            return await _connection.ExecuteAsync(sql, entity, _transaction);
        }

        public async Task<int> DeleteAsync(Dictionary<string, object> keys)
        {
            var where = string.Join(" AND ", keys.Keys.Select(k => $"{k} = @{k}"));
            var sql = $"DELETE FROM {_tableName} WHERE {where}";

            return await _connection.ExecuteAsync(sql, keys, _transaction);
        }

        public async Task<IEnumerable<T>> ExecuteStoredProcedureAsync(string spName, object parameters)
        {
            return await _connection.QueryAsync<T>(
                spName,
                parameters,
                transaction: _transaction,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<TResult>> ExecuteStoredProcedureAsync<TResult>(string spName, object parameters)
        {
            return await _connection.QueryAsync<TResult>(
                spName,
                parameters,
                transaction: _transaction,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
