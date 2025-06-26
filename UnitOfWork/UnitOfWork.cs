using SolucionDA.DatabaseAccess;
using SolucionDA.Models;
using SolucionDA.Repositories;
using System.Data;
using System.Threading.Tasks;

namespace SolucionDA.UnitOfWork;

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