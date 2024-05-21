using DataAccessObject.SQLite.Utils;
using Microsoft.Data.Sqlite;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
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

    private const string databaseName = "databaseSample.db";
    /// <summary>
    /// 
    /// </summary>
    public void SynchronizeTables()
    {
        try
        {
            //CREATE DATABASE FILE
            FileUtils.CopyFromResouceIfNotExists(typeof(Database), databaseName, ConnectionString);

            using ( var connection = new SqliteConnection("Data Source=" + ConnectionString) )
            {
                try
                {
                    connection.Open();

                    IEnumerable<string> listDatabaseTables = RecoversDatabaseTables(connection);
                    IEnumerable<string> listExistingTables = Tables.Keys.Select(x => x.Name);

                    IEnumerable<string> listMissingTables = listDatabaseTables.Except(listExistingTables);

                    foreach ( var entityType in Tables.Keys.Where(x => !listMissingTables.Contains(x.Name)))
                    {
                        var tableName = entityType.Name;
                        if ( !listDatabaseTables.Contains(tableName) )
                        {
                            //THE TABLE DOES NOT EXIST IN THE DATABASE SO THE CREATE COMMAND WILL BE EXECUTED
                            CreatTable(entityType, connection);
                            continue;
                        }

                    }

                    connection.Close();
                }
                catch ( Exception ex )
                {
                    connection.Close();
                    throw;
                }
            }
        }
        catch ( Exception ex )
        {
            Debug.WriteLine(ex);
            throw ex;
        }

    }

    void CreatTable(Type entityType, SqliteConnection connection)
    {
        var tableName = entityType.TableName();
        var properties = entityType.GetProperties();

        var createTableStatement = $"CREATE TABLE {tableName} (";

        for ( int i = 0; i < properties.Length; i++ )
        {
            var property = properties[i];

            var columnName = property.ColumnName();
            var columnType = property.ColumnTypeName();

            if ( i > 0 )
                createTableStatement += ", ";

            createTableStatement += $"{columnName} {columnType}";

            if (property.IsPrimaryKey())
            {
                createTableStatement += " PRIMARY KEY";
            }
            else 
            {
                if (property.HasAttribute<RequiredAttribute>())
                    createTableStatement += " NOT NULL";
                else
                    createTableStatement += " NULL";
            }
        }

        createTableStatement += ")";

        using ( var command = new SqliteCommand(createTableStatement, connection) )
        {
            command.ExecuteNonQuery();
        }
    }

    List<string> RecoversDatabaseTables(SqliteConnection connection)
    {
        var tables = new List<string>();
        string sql = "SELECT NAME FROM SQLITE_MASTER WHERE TYPE = 'TABLE'";

        using ( var command = new SqliteCommand(sql, connection) )
        {
            using ( var reader = command.ExecuteReader() )
            {
                while ( reader.Read() )
                {
                    tables.Add(reader.GetString(0));
                }
            }
        }

        return tables;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Database CreateDatabase() => 
        new Database(ConnectionString, this);


}
