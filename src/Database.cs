using Microsoft.Data.Sqlite;

namespace DataAccessObject.SQLite;

public sealed class Database 
{
    internal SqliteConnection _Connection { get; set; }

    private readonly IConfigurationProvider _configurationProvider;

    public Database(string databasePath, IConfigurationProvider configuration)
    {
        CarregarDatabase(databasePath);
        _configurationProvider = configuration;
    }

    protected void CarregarDatabase(string databasePath)
    {
        string ConnectionString = "Data Source=" + databasePath;

        _Connection = new SqliteConnection(ConnectionString);
        _Connection.Open();
    }

    public void Dispose()
    {
        _Connection.Close();
        _Connection.Dispose();
    }
}
