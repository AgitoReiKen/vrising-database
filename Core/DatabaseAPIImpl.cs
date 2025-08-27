using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Database.Core;
using Database.Utils;
using MySqlConnector.Logging;

namespace Database.API;

public class DatabaseAPIImpl : IDatabaseAPI
{
    public Config Config = Core.Config.Load();

    public DatabaseAPIImpl()
    { 
        if (Config.EnableMysqlLogger)
        {
            MySqlConnectorLogManager.Provider = new MysqlLoggerProvider();
        }

        using (Tests.GeneralTest test = new())
        {
            test.Start(Config);
        }
        CheckConnections();
    }

    void CheckConnections()
    {
        foreach (var kv in Config.Connectors)
        {
            using var con = kv.Value.GetConnection();
            try
            {
                con.Open();
                Log.Info($"[{kv.Key}] Successfully connected");
            }
            catch (Exception ex)
            {
                Log.Error($"[{kv.Key}] Failed to connect");
                Log.Error(ex.Message);
            } 
            con.Close();
        }
    }
    
    public DbConnection GetConnection(string id)
    {
        Log.Debug($"GetConnection {id}");
        string connectorId;

        if (id == null)
        {
            connectorId = Config.DefaultConnector;
            if (connectorId != null && Config.Connectors.ContainsKey(connectorId))
            {
                return Config.Connectors[connectorId].GetConnection();
            }
            Log.Error($"Couldn't provide database, because default connector doesn't exist");
            return null;
        }
         
        if (!Config.Mappings.TryGetValue(id, out connectorId))
        {
            Log.Debug($"GetConnection Mapping not found, using DefaultConnector {Config.DefaultConnector}");
            connectorId = Config.DefaultConnector;
        }
        Log.Debug($"GetConnection ConnectorId {connectorId}");
        
        if (connectorId == null || !Config.Connectors.ContainsKey(connectorId))
        {
            Log.Error($"Couldn't provide database, because no connector with id \"{connectorId}\" was found");
            return null;
        }
       
        return Config.Connectors[connectorId].GetConnection();
    }
}