using System.Data.Common;

namespace Database.API;

public static class DatabaseAPI
{
    public static DbConnection GetConnection(string id)
    {
        return Database.Plugin.Instance.API.GetConnection(id);
    }
}