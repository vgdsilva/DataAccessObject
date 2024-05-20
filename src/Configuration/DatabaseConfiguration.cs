using DataAccessObject.SQLite.Utils;
using Microsoft.Data.Sqlite;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
            FileUtils.CopyResourceMobileIfNotExists(typeof(Database), databaseName, ConnectionString);

            using ( var connection = new SqliteConnection("Data Source=" + ConnectionString) )
            {
                try
                {
                    connection.Open();

                    var databaseExistinTables = GetDatabaseTables(connection);
                    foreach ( var entityType in Tables.Keys )
                    {
                        var tableName = entityType.Name;
                        if ( !databaseExistinTables.Contains(tableName) )
                        {
                            //Create table
                            CreatTable(entityType, connection);
                            continue;
                        }

                        var tableProperties = GetTableProperties(connection, tableName);
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

            throw ex;
        }

    }

    PropertyInfo[] GetTableProperties(SqliteConnection connection, string tableName)
    {
        var properties = new List<PropertyInfo>();
        string sql = $"SELECT name, type FROM sqlite_master JOIN pragma_table_info(name, '{tableName}') ON sqlite_master.name = pragma_table_info.name";

        using (var command = new SqliteCommand(sql, connection))
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var name = reader.GetString(0);
                    var typeName = reader.GetString(1);
                    var propertyType = GetPropertyTypeFromSqliteType(typeName);
                }
            }
        }

        return properties.ToArray();
    }

    Type GetPropertyTypeFromSqliteType(string sqliteType)
    {
        switch ( sqliteType.ToLower() )
        {
            case "integer":
                return typeof(int);
            case "bigint":
                return typeof(long);
            case "real":
                return typeof(float);
            case "numeric":
            case "decimal":
                return typeof(decimal);
            case "text":
            case "varchar":
            case "nvarchar":
                return typeof(string);
            case "blob":
                return typeof(byte[]);
            case "boolean":
                return typeof(bool);
            case "datetime":
                return typeof(DateTime);
            default:
                throw new ArgumentException($"Tipo de dados SQLite não suportado: {sqliteType}");
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

    string GetSqliteTypeFromPropertyType(Type propertyType)
    {
        switch ( propertyType.Name.ToLower() )
        {
            case "int":
            case "short":
            case "byte":
                return "INTEGER";
            case "long":
                return "BIGINT";
            case "float":
            case "double":
                return "REAL";
            case "decimal":
                return "NUMERIC";
            case "string":
                return "TEXT";
            case "bool":
                return "BOOLEAN";
            case "datetime":
                return "DATETIME";
            case "guid":
                return "UNIQUEIDENTIFIER";
            default:
                throw new ArgumentException($"Tipo de dados C# não suportado: {propertyType.Name}");
        }
    }

    List<string> GetDatabaseTables(SqliteConnection connection)
    {
        var tables = new List<string>();

        using ( var command = new SqliteCommand("SELECT name FROM sqlite_master WHERE type = 'table'", connection) )
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
