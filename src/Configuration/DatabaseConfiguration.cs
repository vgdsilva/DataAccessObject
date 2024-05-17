using DataAccessObject.SQLite.Configuration;

namespace DataAccessObject.SQLite;

public interface IConfigurationProvider
{

}

public interface IDatabaseConfiguration
{

}

public sealed class DatabaseConfiguration
{
    public DatabaseConfiguration(DatabaseConfigurationExpression configurationExpression)
    {
        
    }

    public DatabaseConfiguration(string databasePath, Action<DatabaseConfigurationExpression> configure) : this(Build(configure)) { }

    static DatabaseConfigurationExpression Build(Action<DatabaseConfigurationExpression> configure)
    {
        DatabaseConfigurationExpression expr = new();
        configure(expr);
        return expr;
    }
}
