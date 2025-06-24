using Il2CppSystem.Diagnostics;

namespace Database.Utils;

public static class StopwatchExtensions
{
    public static long ElapsedNanoseconds(this Stopwatch stopwatch)
    {
        return (long)(stopwatch.ElapsedTicks * (1_000_000.0 / Stopwatch.Frequency));
    }
}