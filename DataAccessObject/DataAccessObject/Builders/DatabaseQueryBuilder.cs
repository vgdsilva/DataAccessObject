using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessObject.Builders
{
    public abstract class DatabaseQueryBuilder
    {
        protected Type TableType { get; set; }

        public abstract string GenerateSQL();

        protected DatabaseQueryBuilder(Type tableType)
        {
            TableType = tableType ?? throw new ArgumentNullException();
        }

        public static DatabaseQueryBuilder Select(Type tableType) => new SelectQueryBuilder(tableType);

        public static DatabaseQueryBuilder CreateTable(Type tableType) => new TableQueryBuilder(tableType);

        public static DatabaseQueryBuilder Insert(Type tableType) => new InsertQueryBuilder(tableType);

        public static DatabaseQueryBuilder Update(Type tableType) => new UpdateQueryBuilder(tableType);


        internal class SelectQueryBuilder : DatabaseQueryBuilder
        {
            private List<string> Fields { get; set; }
            private string TableName { get; set; }
            private List<string> Conditions { get; set; }
            private List<string> Joins { get; set; }

            public SelectQueryBuilder(Type tableType) : base(tableType)
            {
                Fields = new List<string>();
                Conditions = new List<string>();
                Joins = new List<string>();
                TableName = tableType.Name; // Assuming tableType.Name is the table name. You might need a more sophisticated way to get the table name.
            }

            public SelectQueryBuilder AddField(string name, string tableFrom = null, string alias = null)
            {
                // Format the field with optional table and alias
                Fields.Add(string.Format("{0}{1} AS {2}",
                    string.IsNullOrEmpty(tableFrom) ? string.Empty : $"{tableFrom}.",
                    name,
                    string.IsNullOrEmpty(alias) ? name : alias));
                return this;
            }

            public SelectQueryBuilder AddCondition(string condition)
            {
                Conditions.Add(condition);
                return this;
            }

            public SelectQueryBuilder AddInnerJoin(string table, string alias, string tableFrom, string fieldFromTableFrom, string tableTo, string fieldFromTableTo)
            {
                Joins.Add(BuildJoinString("INNER", table, alias, tableFrom, fieldFromTableFrom, tableTo, fieldFromTableTo));
                return this;
            }

            string BuildJoinString(string type, string table, string alias, string tableFrom, string fieldFromTableFrom, string tableTo, string fieldFromTableTo)
            {
                return string.Format("{0} JOIN {1} {2} ON {3}{4} = {5}{6}",
                    type,
                    table,
                    string.IsNullOrEmpty(alias) ? table : alias,
                    tableFrom,
                    fieldFromTableFrom,
                    tableTo,
                    fieldFromTableTo);
            }

            public override string GenerateSQL()
            {
                if (Fields.Count == 0)
                {
                    throw new InvalidOperationException("No fields specified for SELECT query.");
                }

                StringBuilder sb = new StringBuilder();

                // Build SELECT clause
                sb.Append("SELECT ");
                sb.Append(string.Join(", ", Fields));

                // Build FROM clause
                sb.AppendLine(" FROM ");
                sb.Append(TableName);

                // Build JOIN clauses
                if (Joins.Count > 0)
                {
                    sb.AppendLine(" ");
                    sb.Append(string.Join(" ", Joins));
                }

                // Build WHERE clause
                if (Conditions.Count > 0)
                {
                    sb.AppendLine(" WHERE ");
                    sb.Append(string.Join(" AND ", Conditions));
                }

                return sb.ToString();
            }
        }

        internal class TableQueryBuilder : DatabaseQueryBuilder
        {
            List<string> Columns { get; set; }
            string PK { get; set; }
            List<string> FKs { get; set; }

            public TableQueryBuilder(Type tableType) : base(tableType)
            {
                PK = string.Empty;
                Columns = new List<string>();
                FKs = new List<string>();
            }

            public TableQueryBuilder AddPrimaryKey(string primaryKeyName, string primaryKeyType)
            {
                Columns.Add(CreateSQLColumn(primaryKeyName, primaryKeyType));
                PK = primaryKeyName;

                if (string.IsNullOrEmpty(PK))
                    throw new ArgumentNullException("PK is null");

                return this;
            }

            public TableQueryBuilder AddColumn(string columnName, string columnType)
            {
                Columns.Add(CreateSQLColumn(columnName, columnType));
                return this;
            }

            string CreateSQLColumn(string columnName, string columnType)
            {
                return $@"{columnName} {columnType}";
            }

            public TableQueryBuilder AddForeignKey(string columnName, string referencedTable, string referencedColumn)
            {
                FKs.Add(CreateSQLFK(columnName, referencedTable, referencedColumn));
                return this;
            }

            string CreateSQLFK(string columnName, string referencedTable, string referencedColumn)
            {
                return $@"FOREIGN KEY ({columnName}) REFERENCES {referencedTable}({referencedColumn})";
            }

            public override string GenerateSQL()
            {
                StringBuilder builder = new StringBuilder();

                builder.AppendLine($@"CREATE TABLE {TableType.Name} (");

                foreach (var column in Columns)
                    builder.AppendLine($@"  {column},");

                builder.AppendLine($@"  PRIMARY KEY({PK})");

                if (FKs.Count > 0)
                {
                    builder.Append(",");

                    int countFKs = 0;
                    foreach (var FK in FKs)
                    {
                        countFKs++;
                        if (countFKs > 1)
                            builder.Append(",");

                        builder.AppendLine($@"  {FK}");
                    }
                }

                builder.Append(")");

                return builder.ToString();
            }
        }

        internal class InsertQueryBuilder : DatabaseQueryBuilder
        {
            Dictionary<string, object> ColumnValues { get; set; }

            public InsertQueryBuilder(Type tableType) : base(tableType)
            {
                ColumnValues = new Dictionary<string, object>();
            }

            public InsertQueryBuilder AddField(string columnName, object columnValue)
            {
                ColumnValues.Add(columnName, columnValue);

                return this;
            }

            public override string GenerateSQL()
            {
                StringBuilder builder = new StringBuilder();

                List<string> columns = new List<string>();
                List<object> values = new List<object>(); ;

                foreach (var ColumnAndValue in ColumnValues)
                {
                    columns.Add(ColumnAndValue.Key);
                    values.Add(ColumnAndValue.Value);
                }

                builder.AppendLine($@"INSERT INTO {TableType.Name} ({string.Join(",", columns)})");
                builder.AppendLine($@"VALUES ({string.Join(",", values)})");

                return builder.ToString();
            }
        }

        public class UpdateQueryBuilder : DatabaseQueryBuilder
        {
            Dictionary<string, object> FieldsToSet { get; set; }
            Dictionary<string, object> Wheres { get; set; }


            public UpdateQueryBuilder(Type tableType) : base(tableType)
            {
                FieldsToSet = new Dictionary<string, object>();
                Wheres = new Dictionary<string, object>();
            }

            public UpdateQueryBuilder SetField(string columnName, object columnValue)
            {
                FieldsToSet.Add(columnName, columnValue);
                return this;
            }

            public UpdateQueryBuilder Where(string columnName, object columnValue)
            {
                Wheres.Add(columnName, columnValue);
                return this;
            }

            public override string GenerateSQL()
            {
                StringBuilder builder = new StringBuilder();

                if (Wheres.Count == 0)
                    throw new Exception("Update without WHERE clause");

                builder.AppendLine($@"UPDATE {TableType.Name} SET {String.Join(", ", FieldsToSet.Select(x => string.Format("{0} = {1}", x.Key, x.Value)))}");

                if (Wheres.Count > 0)
                    builder.Append($" WHERE {String.Join(" AND ", Wheres.Select(x => string.Format("{0} = {1}", x.Key, x.Value)))}");

                return builder.ToString();
            }
        }
    }
}
