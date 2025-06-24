using System;
using System.IO;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using Database.API;
using Database.Utils;
using MySqlConnector.Logging;
using Newtonsoft.Json.Linq;
using Logger = BepInEx.Logging.Logger;

namespace Database;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    public static Plugin Instance;
    public DatabaseAPI API; 
    public override void Load()
    {
        Instance = this;
        var metadata = MetadataHelper.GetMetadata(this);
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loading...");

        var configPath = $"{Paths.ConfigPath}/Database/config.json";
        string configText = null;
        try
        {
            configText = File.ReadAllText(configPath);
        }
        catch (Exception ex)
        {
           Utils.Log.Error($"Couldn't read config at {configPath}");
           throw;
        }

        JObject json = null;
        try
        {
            json = JObject.Parse(configText);
        }
        catch (Exception ex)
        {
            Utils.Log.Error("Couldn't parse config");
            throw;
        }

        Config config = new Config(json);
        if (config.EnableMysqlLogger)
        {
            MySqlConnectorLogManager.Provider = new MysqlLoggerProvider();
        }

        using (Tests.GeneralTest test = new())
        {
            test.Start(config);
        }
        
        API = new DatabaseAPI(config);
        
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

    }

    public override bool Unload()
    {
        return true;
    } 
}
