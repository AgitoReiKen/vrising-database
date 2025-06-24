using System;
using MySqlConnector.Logging;

namespace Database.Utils;

public class MysqlLoggerProvider : IMySqlConnectorLoggerProvider
{
    public IMySqlConnectorLogger CreateLogger(string name) => new MysqlBepInExLogger(name);
}

public class MysqlBepInExLogger : IMySqlConnectorLogger
{
    private readonly string _name;

    public MysqlBepInExLogger(string name) {
        _name = name;
    }

    public bool IsEnabled(MySqlConnectorLogLevel level)
    {
        // Optional: Filter by log level
        return true;
    }

    public void Log(MySqlConnectorLogLevel level, int eventId, object state, Exception exception = null)
    {
        string message = $"[MySqlConnector] [{level}] [{_name}] {state}";
        if (exception != null)
        {
            message += $"\nException: {exception}";
        }

        Utils.Log.Debug(message);
    }

    public void Log(MySqlConnectorLogLevel level, string message, object[] args = null, Exception exception = null)
    {
        string formattedMessage = args != null ? string.Format(message, args) : message;
        formattedMessage = $"[MySqlConnector] [{level}] [{_name}] {formattedMessage}";

        if (exception != null)
        {
            formattedMessage += $"\nException: {exception}";
        }

        Utils.Log.Debug(formattedMessage);
    }
}