using DataAccessObject.SQLite.Interfaces;
using Microsoft.Data.Sqlite;

namespace DataAccessObject.SQLite;

public sealed class Database : IDatabase
{
    internal SqliteConnection _Connection { get; set; }

    private readonly DatabaseConfiguration _configuration;

    public string SQL { get; set; } = string.Empty;

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
        SQL = string.Empty;

        _Connection.Close();
        _Connection.Dispose();
    }

    public void Execute()
    {
        try
        {
            using ( var command = new SqliteCommand(SQL, _Connection) )
            {
                command.ExecuteNonQuery();
            }
        }
        catch ( Exception ex )
        {
            throw;
        }
    }

    public void AddParameter(string name, object value)
    {
        throw new NotImplementedException();
    }

    public async Task<List<T>> QueryAsync<T>()
    {
        using ( var command = new SqliteCommand(SQL, _Connection) )
        {
            using ( var reader = await command.ExecuteReaderAsync() )
            {
                var results = new List<T>();
                var properties = typeof(T).GetProperties();

                while ( reader.Read() )
                {
                    var item = Activator.CreateInstance<T>();

                    foreach ( var property in properties )
                    {
                        var columnName = property.Name;
                        var columnIndex = reader.GetOrdinal(columnName);

                        if (reader.IsDBNull(columnIndex))
                        {
                            continue;
                        }

                        var value = reader.GetValue(columnIndex);
                        property.SetValue(item, value);
                    }

                    results.Add(item);
                }

                return results;
            }
        }
    }
}

