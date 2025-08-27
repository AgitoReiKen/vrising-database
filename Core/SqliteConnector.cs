using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Database.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using Database.Utils;

namespace Database.Core;

 
public class SqliteConnector : IDatabaseConnector
{
    public string ConnectionString; 
    public string Path;
    public SqliteConnector(JObject json)
    {
        Path = (string)json["Path"];
        ConnectionString = $"Data Source={Path}"; 
        CreateDatabase();
    }

    private void CreateDatabase()
    {
        if (!File.Exists(Path))
        {
            try
            {
                File.Create(Path).Close();
                Log.Info($"[SqliteDatabase] Database created at {Path}");
            }
            catch (Exception)
            {
                Log.Error($"[SqliteDatabase] Couldn't create database at {Path}");
                throw;
            }
        }
    }

    public DbConnection GetConnection()
    {
        return new SQLiteConnection(ConnectionString);
    }
}