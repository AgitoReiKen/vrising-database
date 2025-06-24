using BepInEx.Logging;
using Il2CppSystem;

namespace Database.Utils;

public class AdvancedManualLogSource : ManualLogSource
{
    private const string Green = "\u001b[32m";
    private const string Reset = "\u001b[0m";

    public AdvancedManualLogSource(string sourceName) : base(sourceName) { }
    
    public void Log(LogLevel level, object data)
    {
        if (level == LogLevel.Info)
        {
            string prefix = $"{Green}[Info : {SourceName}]{Reset}";
            string message = $"{Green}{data}{Reset}";
            Console.WriteLine($"{prefix} {message}");
        }
        else
        {
            base.Log(level, data);
        }
    }
}