using System.Reflection;

namespace DataAccessObject.SQLite;

public sealed class DatabaseConfiguration
{

    public string ConnectionString { get; private set; }
    readonly Dictionary<Type, PropertyInfo[]> Tables = new();

    public DatabaseConfiguration(string databasePath, Action<DatabaseConfiguration> configureTables)
    {
        ConnectionString = databasePath;
        configureTables(this);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public void RegisterTable<TEntity>()
    {
        var entityType = typeof(TEntity);

        if (!Tables.ContainsKey(entityType) )
        {
            var tableProperties = entityType.GetProperties();
            Tables.Add(entityType, tableProperties);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Database CreateDatabase() => 
        new Database(ConnectionString, this);


}
