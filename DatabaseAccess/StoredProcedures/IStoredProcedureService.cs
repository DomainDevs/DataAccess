using Microsoft.Data.SqlClient;
using System.Data;

namespace SolucionDA.DatabaseAccess.StoredProcedures
{
    public interface IStoredProcedureService
    {
        DataSet ExecuteToDataSet(string spName, SqlConnection connection, object parameters);
    }
}
