using System.Data.Common;
using System.Data.SQLite;
using Database.API;

namespace Database.Core;

public interface IDatabaseConnector {
    public DbConnection GetConnection(); 
}