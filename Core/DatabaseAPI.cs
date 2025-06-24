using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Database.Core;
using Database.Utils;

namespace Database.API;

public class DatabaseAPI : IDatabaseAPI 
{
    public static DatabaseAPI Instance => Plugin.Instance.API; 

    public Config Config;

    public DatabaseAPI(Config config)
    {
        Config = config;
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
    
    public DbConnection GetConnection(string pluginId)
    {
        string connectorId;
        
        if (!Config.Plugins.TryGetValue(pluginId, out connectorId))
        {
            connectorId = Config.DefaultConnector;
        }

        if (connectorId == null)
        {
            Log.Error($"Couldn't provide database for mod {pluginId}, because no connectors found for it (default is not configured)");
        }
        
        if (!Config.Connectors.ContainsKey(connectorId))
        {
            Log.Error($"Couldn't provide database for mod {pluginId}, because connector with id {connectorId} is not found");
        }
       
        return Config.Connectors[connectorId].GetConnection();
    }
}