using Microsoft.Data.Sqlite;

namespace DataAccessObject.SQLite;

public sealed class Database 
{
    internal SqliteConnection _Connection { get; set; }

    private readonly DatabaseConfiguration _configuration;

    public Database(string databasePath, DatabaseConfiguration configuration)
    {
        CarregarDatabase(databasePath);
        _configuration = configuration;
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
