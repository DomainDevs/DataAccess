using System.Data;

namespace SolucionDA.DatabaseAccess
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
