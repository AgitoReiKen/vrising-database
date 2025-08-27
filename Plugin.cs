using System;
using System.IO;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using System.Reflection;
using Database.API;
using Database.Core;
using Database.Utils;
using MySqlConnector.Logging;
using Newtonsoft.Json.Linq;
using Logger = BepInEx.Logging.Logger;

namespace Database;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    public static Plugin Instance;
    public IDatabaseAPI API; 
    public override void Load()
    {
        Instance = this;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loading...");
  
        API = new DatabaseAPIImpl();
        
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

    }

    public override bool Unload()
    {
        return true;
    } 
}
