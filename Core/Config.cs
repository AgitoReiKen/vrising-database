using System.Collections.Generic;
using Database.Core;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using BepInEx;
using Database.Utils;
using Il2CppSystem.Linq;

namespace Database.Core;
 
public class Config
{ 
    public Dictionary<string, IDatabaseConnector> Connectors;
    public string DefaultConnector;
    public Dictionary<string, string> Mappings;
    public bool EnableMysqlLogger;
    public Config(JObject json)
    {
        Connectors = new();
        Mappings = new();
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
                else
                {
                    Log.Error($"Database Type {type} is not supported");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Couldn't parse Connectors -> {key}.");
                Log.Error($"{ex.Message}");
            }
        }
        if (json.TryGetValue("DefaultConnector", out JToken defaultConnector) && Connectors.ContainsKey((string)defaultConnector))
        {
            DefaultConnector = (string)defaultConnector;
        }
        else
        {
            Log.Warning("DefaultConnector either is not set or invalid. Mappings that are not explicitly set - will not get a database connector.");
        }

        var mappings = (json["Mappings"].Cast<JObject>()).Properties().ToList();
        foreach (var prop in mappings)
        {
            var key = prop.Name;
            var connectorId = (string)prop.Value;
            if (!Connectors.ContainsKey(connectorId))
            {
                Log.Error($"Mapping {key} uses connector that doesn't exists - {connectorId}");
                throw new System.Exception("Invalid configuration.");
            }
            
            Mappings.Add(key, connectorId);
        }

        if (json.TryGetValue("EnableMysqlLogger", out JToken enableMysqlLogger))
        {
            EnableMysqlLogger = (bool)enableMysqlLogger;
        }
    }
    public static Config Load()
    {
        var configPath = $"{Paths.ConfigPath}/{MyPluginInfo.PLUGIN_GUID}/config.json";
        string configText;
        try
        {
            configText = File.ReadAllText(configPath);
        }
        catch (Exception)
        {
            Log.Error($"Couldn't read config at {configPath}");
            throw;
        }

        JObject json;
        try
        {
            json = JObject.Parse(configText);
        }
        catch (Exception)
        {
            Log.Error("Couldn't parse config");
            throw;
        }

        return new Config(json);
    }
}