using System;
using System.Data.Common;

namespace Database.API;

public static class DatabaseExtensions
{
    /*
     * 
     */
    public static DbCommand Query(this DbConnection db, string query, params object[] parameters)
    {
        DbCommand command = db.CreateCommand();
        command.CommandText = query;

        if (parameters != null)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                DbParameter param = command.CreateParameter();
                param.ParameterName = $"@p{i}";
                param.Value = parameters[i] ?? DBNull.Value;
                command.Parameters.Add(param);
            }
        }

        return command;
    }   
}