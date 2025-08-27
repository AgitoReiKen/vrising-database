using System;
using System.Collections.Generic;
using System.Data.Common;
using Database.API;
using Database.Core;
using Database.Utils;
using Il2CppSystem.Diagnostics;

namespace Database.Tests;

public class GeneralTest : IDisposable
{
    private Dictionary<string, DbConnection> Connections = new();
    public void Start(Config config)
    {
        foreach (var connector in config.Connectors)
        {
            
            try
            {
                var con = connector.Value.GetConnection();
                con.Open();
                Connections.Add(connector.Key, con);
            }
            catch (Exception ex)
            {
                Log.Error($"[GeneralTest] Database {connector.Key} failed to connect");
                Log.Error(ex.Message);
            }
        }
        
        Dictionary<string, Action<DbConnection>> stages =
            new Dictionary<string, Action<DbConnection>>();
        stages.Add("DropTableIfExists", DropTableIfExists);
        stages.Add("CreateTable", TestCreateTable);
        stages.Add("Insert", TestInsert);
        stages.Add("Update", TestUpdate);
        stages.Add("GetUpdated", TestGetUpdated);
        stages.Add("Delete", TestDelete);
        stages.Add("DropTable", TestDropTable);
        
        int perfIterations = 10;
        foreach (var connection in Connections)
        {
            Dictionary<string, long> stagesPerf =
                new Dictionary<string, long>();
            stagesPerf.Add("DropTableIfExists", 0);
            stagesPerf.Add("CreateTable", 0);
            stagesPerf.Add("Insert", 0);
            stagesPerf.Add("Update", 0);
            stagesPerf.Add("GetUpdated", 0);
            stagesPerf.Add("Delete", 0);
            stagesPerf.Add("DropTable", 0);

            bool hadErrors = false;

            foreach (var stage in stages)
            {
                try
                {
                    stage.Value.Invoke(connection.Value);
                }
                catch (Exception ex)
                {
                    hadErrors = true;
                    Log.Error($"[{stage.Key}] [{connection.Key}] failed. Exception:");
                    Log.Error(ex.Message);
                    
                    // Don't continue
                    break;
                }
            }

            // Don't do performance if had errors
            if (hadErrors)
            {
                Log.Error($"Connection [{connection.Key}] failed some tests.");
                continue;
            }

            for (int i = 0; i < perfIterations; ++i)
            {
                foreach (var stage in stages)
                {
                    try
                    {
                        var sw = System.Diagnostics.Stopwatch.StartNew();
                        stage.Value.Invoke(connection.Value);
                        sw.Stop();
                         
                        stagesPerf[stage.Key] += sw.ElapsedMilliseconds;
                    }
                    catch (Exception ex)
                    {
                        hadErrors = true;
                        Log.Error($"[{stage.Key}] [{connection.Key}] failed. Exception:");
                        Log.Error(ex.Message);
                    
                        // Don't continue
                        break;
                    }
                }

                if (hadErrors)
                {
                    Log.Error($"Connection [{connection.Key}] failed performance test.");
                    break;
                }
            }

            foreach (var stage in stagesPerf)
            {
                var ms = (double)stage.Value / (double)perfIterations;

                if (ms > 3.0)
                {
                    Log.Warning($"[{connection.Key}] [{stage.Key}] avg {ms}ms");
                }
                else
                {
                    Log.Info($"[{connection.Key}] [{stage.Key}] avg {ms}ms");
                }
            }
        }
         
    }

    public void RunTestsFor(string id, DbConnection db)
    {
        
    }
    public void Dispose()
    {
        foreach (var db in Connections)
        {
            db.Value.Dispose();
        }
        Connections.Clear();
    }
    void TestCreateTable(DbConnection db)
    {

        using var q = db.Query($"CREATE TABLE IF NOT EXISTS VRISING_TEST (" +
                               $"id INT AUTO_INCREMENT PRIMARY KEY," +
                               $"data1 VARCHAR(255)," +
                               $"data2 INT" +
                               $")");


        var r = q.ExecuteNonQuery();
    }

    void TestInsert(DbConnection db)
    {
        using var q = db.Query("INSERT INTO VRISING_TEST (data1, data2) VALUES(@p0, @p1)", "Inserted", 12345);
        var r = q.ExecuteNonQuery();
    }

    void TestUpdate(DbConnection db)
    {
        using var cmd = db.Query("UPDATE VRISING_TEST SET data1=@p0 WHERE data2=@p1", "Updated", 12345);
        var r = cmd.ExecuteNonQuery();
    }

    void TestGetUpdated(DbConnection db)
    {
        using var cmd = db.Query("SELECT * FROM VRISING_TEST WHERE data1 LIKE @p0", "Updated");
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            var data2 = r.GetInt32(r.GetOrdinal("data2"));
            if (data2 == 12345)
            {
                break;
            }
        }
    }
    void TestDelete(DbConnection db)
    {
        using var cmd = db.Query("DELETE FROM VRISING_TEST");
        var r = cmd.ExecuteNonQuery();
    }

    void TestDropTable(DbConnection db)
    {
        using var cmd = db.Query("DROP TABLE VRISING_TEST");
        var r = cmd.ExecuteNonQuery();
    }
    void DropTableIfExists(DbConnection db)
    {
        using var cmd = db.Query("DROP TABLE IF EXISTS VRISING_TEST");
        var r = cmd.ExecuteNonQuery();
    }
}