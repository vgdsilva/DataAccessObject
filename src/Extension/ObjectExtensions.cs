using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.SQLite;

public static class ObjectExtensions
{
    public static bool HasAttribute<TAttribute>(this PropertyInfo propertyInfo)
        where TAttribute : Attribute
    {
        return propertyInfo.GetCustomAttribute<TAttribute>() != null;
    }


    public static bool HasAttribute<TAttribute>(this Type type)
        where TAttribute : Attribute
    {
        return type.GetCustomAttribute<TAttribute>() != null;
    }

    public static string TableName(this Type type)
    {
        if (type.HasAttribute<TableAttribute>())
        {
            return type.GetCustomAttribute<TableAttribute>().Name;
        }

        return type.Name;
    }

    public static bool IsPrimaryKey(this PropertyInfo? type)
    {
        if (type.HasAttribute<KeyAttribute>())
        {
            return true;
        }

        return false;
    }

    public static string ColumnName(this PropertyInfo? type)
    {
        if ( type.HasAttribute<ColumnAttribute>() )
        {
            ColumnAttribute Attribute = type.GetCustomAttribute<ColumnAttribute>();
            return Attribute.Name;
        }

        return type.Name;
    }

    public static string ColumnTypeName(this PropertyInfo? type)
    {
        if ( type.HasAttribute<ColumnAttribute>() )
        {
            ColumnAttribute Attribute = type.GetCustomAttribute<ColumnAttribute>();
            return Attribute.TypeName;
        }

        switch ( type.PropertyType.Name.ToLower() )
        {
            case "int":
            case "short":
            case "byte":
                return "INTEGER";
            case "long":
            case "int64":
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
                throw new ArgumentException($"Tipo de dados C# não suportado: {type.Name}");
        }
    }
}
