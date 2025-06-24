using System.Collections.Generic;
using Database.Core;
using Newtonsoft.Json.Linq;
using System;
using Database.Utils;
using Il2CppSystem.Linq;

namespace Database;
 
public class Config
{ 
    public Dictionary<string, IDatabaseConnector> Connectors;
    public string DefaultConnector;
    public Dictionary<string, string> Plugins;
    public bool EnableMysqlLogger;
    public Config(JObject json)
    {
        Connectors = new();
        Plugins = new();
        DefaultConnector = null;
        EnableMysqlLogger = false;
        var connectors = (json["Connectors"].Cast<JObject>()).Properties().ToList();
        foreach (var prop in connectors)
        {
            var key = prop.Name;
            try
            {
                var type = prop.Value.Value<string>("Type");
                if (type == "Mysql")
                {
                    var connector = new MysqlConnector(prop.Value.Value<JObject>(type));
                    Connectors.Add(key, connector);
                }
                else if (type == "Sqlite")
                {
                    var connector = new SqliteConnector(prop.Value.Value<JObject>(type));
                    Connectors.Add(key, connector);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Couldn't parse Connectors -> {key}.");
                Log.Error($"{ex.Message}");
            }
        }
        if (!json.TryGetValue("DefaultConnector", out JToken defaultConnector) || !Connectors.ContainsKey((string)defaultConnector))
        {
            Log.Warning("DefaultConnector either is not set or invalid. Plugins that are not explicitly set, will not get a database connector.");
        }

        var plugins = (json["Plugins"].Cast<JObject>()).Properties().ToList();
        foreach (var prop in plugins)
        {
            var key = prop.Name;
            var connectorId = (string)prop.Value;
            if (!Connectors.ContainsKey(connectorId))
            {
                Log.Error($"Plugin {key} uses connector that doesn't exists - {connectorId}");
                throw new System.Exception("Invalid configuration.");
            }
            
            Plugins.Add(key, connectorId);
        }

        if (json.TryGetValue("EnableMysqlLogger", out JToken enableMysqlLogger))
        {
            EnableMysqlLogger = (bool)enableMysqlLogger;
        }
    }
}