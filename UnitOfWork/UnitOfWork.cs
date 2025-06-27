using SolucionDA.DatabaseAccess;
using SolucionDA.Models;
using SolucionDA.Repositories;
using System.Data;
using System.Threading.Tasks;

namespace SolucionDA.UnitOfWork;

/*
public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    private IDbTransaction _transaction;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(IDbConnection connection)
    {
        _connection = connection;
        _connection.Open();
        _transaction = _connection.BeginTransaction();
    }

    public IRepository<T> GetRepository<T>() where T : class
    {
        var type = typeof(T);

        if (!_repositories.ContainsKey(type))
        {
            var repo = new SqlServerRepository<T>(_connection, _transaction);
            _repositories[type] = repo;
        }

        return (IRepository<T>)_repositories[type];
    }

    public void Commit()
    {
        _transaction?.Commit();
        _transaction = _connection.BeginTransaction();
    }

    public void Rollback()
    {
        _transaction?.Rollback();
        _transaction = _connection.BeginTransaction();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _connection?.Dispose();
    }
}
*/

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnectionFactory _connectionFactory;

    private readonly Dictionary<string, IDbConnection> _connections = new();
    private readonly Dictionary<string, IDbTransaction> _transactions = new();
    private readonly Dictionary<(Type, string), object> _repositories = new();

    public UnitOfWork(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public IRepository<T> GetRepository<T>(string dbAlias = "SqlServer") where T : class
    {
        var key = (typeof(T), dbAlias);

        if (_repositories.ContainsKey(key))
            return (IRepository<T>)_repositories[key]!;

        if (!_connections.ContainsKey(dbAlias))
        {
            var conn = _connectionFactory.CreateConnection(dbAlias);
            conn.Open();
            _connections[dbAlias] = conn;
            _transactions[dbAlias] = conn.BeginTransaction();
        }

        var connection = _connections[dbAlias];
        var transaction = _transactions[dbAlias];

        IRepository<T> repository = dbAlias switch
        {
            "SqlServer" => new SqlServerRepository<T>(connection, transaction),
            "Sybase" => new SybaseRepository<T>(connection, transaction),
            _ => throw new InvalidOperationException($"No hay repositorio registrado para el alias '{dbAlias}'.")
        };

        _repositories[key] = repository;

        return repository;
    }

    public void Commit(string dbAlias = "SqlServer")
    {
        if (_transactions.TryGetValue(dbAlias, out var tx))
        {
            tx.Commit();
            _transactions[dbAlias] = _connections[dbAlias].BeginTransaction();
        }
    }

    public void Rollback(string dbAlias = "SqlServer")
    {
        if (_transactions.TryGetValue(dbAlias, out var tx))
        {
            tx.Rollback();
            _transactions[dbAlias] = _connections[dbAlias].BeginTransaction();
        }
    }

    public void Dispose()
    {
        foreach (var tx in _transactions.Values)
            tx.Dispose();

        foreach (var conn in _connections.Values)
            conn.Dispose();

        _transactions.Clear();
        _connections.Clear();
        _repositories.Clear();
    }
}