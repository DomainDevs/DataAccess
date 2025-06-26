using Microsoft.Data.SqlClient;
using System.Data;

namespace SolucionDA.DatabaseAccess.StoredProcedures;

public class StoredProcedureExecutor
{
    public DataSet ExecuteStoredProcedureToDataSet(string spName, SqlConnection connection, object parameters)
    {
        using var command = new SqlCommand(spName, connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        foreach (var prop in parameters.GetType().GetProperties())
        {
            command.Parameters.AddWithValue("@" + prop.Name, prop.GetValue(parameters));
        }

        var adapter = new SqlDataAdapter(command);
        var dataSet = new DataSet();
        adapter.Fill(dataSet);

        return dataSet;
    }
}
