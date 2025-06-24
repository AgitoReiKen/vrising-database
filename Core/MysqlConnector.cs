using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using BepInEx;
using Database.API;
using Database.Utils;
using Gee.External.Capstone.M68K;
using MySqlConnector;
using MySqlConnector.Logging;
using Newtonsoft.Json.Linq;

namespace Database.Core;
public class MysqlConnector : IDatabaseConnector
{
    public string User;
    public string Password;
    public string Host;
    public string Database;
    public short Port;
    public string ConnectionString;
    public MysqlConnector(JObject json)
    {
        bool createDatabase = false;
        Port = 3306;
        if (!json.TryGetValue("Host", out JToken host))
        {
            throw new System.Exception("Couldn't get 'Host' property required for mysql connection");
        } 
        if (!json.TryGetValue("User", out JToken user))
        {
            throw new System.Exception("Couldn't get 'User' property required for mysql connection");
        }
        if (!json.TryGetValue("Password", out JToken password))
        {
            throw new System.Exception("Couldn't get 'Password' property required for mysql connection");
        }
        if (!json.TryGetValue("Database", out JToken database))
        {
            throw new System.Exception("Couldn't get 'Database' property required for mysql connection");
        }    
        if (json.TryGetValue("CreateDatabase", out JToken _createDatabase))
        {
            createDatabase = (bool)_createDatabase;
        }
        if (json.TryGetValue("Port", out JToken port))
        {
            Port = (short)port;
        }
        Host = (string)host;
        User = (string)user;
        Password = (string)password;
        Database = (string)database;

        if (createDatabase)
        {
            CreateDatabase();
        }
        ConnectionString = $"Server={Host};Port={Port};User ID={User};Password={Password};Database={Database};";
    }

    private void CreateDatabase()
    {
        var tempConnectionString = $"Server={Host};Port={Port};User ID={User};Password={Password};";
        try
        {
            using var tempConnection = new MySqlConnection(tempConnectionString);
            tempConnection.Open();
            using var cmd = tempConnection.CreateCommand();
            cmd.CommandText = $"CREATE DATABASE IF NOT EXISTS {Database};";
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Log.Error($"[MysqlDatabase] [{tempConnectionString}] Couldn't create database on startup");
            Log.Error(ex.Message);
        }
    }

    public DbConnection GetConnection()
    {
        return new MySqlConnection(ConnectionString);
    } 
} 