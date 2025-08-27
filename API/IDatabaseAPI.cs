using System.Data.Common;

namespace Database.API;

public interface IDatabaseAPI
{
    /// <summary>
    /// Retrieves a database connection associated with the MappingId or ConnectorId. 
    /// </summary>
    /// <param name="id">MappingId or ConnectorId. If null, default connector will be chosen</param>
    /// <returns>
    /// A <see cref="DbConnection"/> object that can be used to interact with the database. The caller is responsible for opening the connection and disposing of it when done.
    /// </returns>
    /// <remarks>
    /// The returned connection is not opened by default. You should explicitly call <c>Open()</c> before executing commands.
    /// It's recommended to use a <c>using</c> statement or <c>using var</c> declaration to ensure proper disposal of the connection.
    ///
    /// <para>Usage example:</para>
    /// <code>
    /// using var db = GetConnection("my.plugin.id");
    /// try
    /// {
    ///     db.Open();
    ///     using var cmd = db.Query("INSERT INTO TableName (column) VALUES(@p0);", "parameter0");
    ///     // Execute command, handle result
    /// }
    /// catch (Exception ex)
    /// {
    ///     // Handle errors
    /// }
    /// </code>
    /// <code>
    /// using var db = GetConnection("some-other-connection-id");
    /// try
    /// {
    ///     db.Open();
    ///     using var cmd = db.Query("INSERT INTO TableName (column) VALUES(@p0);", "parameter0");
    ///     // Execute command, handle result
    /// }
    /// catch (Exception ex)
    /// {
    ///     // Handle errors
    /// }
    /// </code>
    /// </remarks>
    public DbConnection GetConnection(string id);
}