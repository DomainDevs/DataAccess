using System;
using SolucionDA.Repositories;


namespace SolucionDA.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> GetRepository<T>() where T : class;
    void Commit();
    void Rollback();
}
