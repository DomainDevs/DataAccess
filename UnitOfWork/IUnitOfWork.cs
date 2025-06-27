using System;
using SolucionDA.Repositories;


namespace SolucionDA.UnitOfWork;

/*
public interface IUnitOfWork : IDisposable
{
    IRepository<T> GetRepository<T>() where T : class;
    void Commit();
    void Rollback();
}
*/

public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Devuelve un repositorio genérico para la entidad especificada y la base de datos indicada.
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    /// <param name="dbAlias">Alias de base de datos (ej. "SqlServer", "LegacySybase")</param>
    IRepository<T> GetRepository<T>(string dbAlias = "SqlServer") where T : class;

    /// <summary>
    /// Confirma la transacción activa en la base de datos indicada.
    /// </summary>
    /// <param name="dbAlias">Alias de base de datos</param>
    void Commit(string dbAlias = "SqlServer");

    /// <summary>
    /// Revierte la transacción activa en la base de datos indicada.
    /// </summary>
    /// <param name="dbAlias">Alias de base de datos</param>
    void Rollback(string dbAlias = "SqlServer");
}