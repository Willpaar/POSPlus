using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Windows.Controls;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Common.Helpers
{
    public static class SQLHelper
    {

        private static readonly string _connectionString;

        static SQLHelper()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private static string GetConnectionString()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Database connection string has not been initialized.");
            }
            return _connectionString;
        }

        private static T UseConnection<T>(Func<SqlConnection, T> getData)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                return getData(connection);
            }
        }

        public static T? QueryFirst<T>(string sql, object? parameters = null)
        {
            return UseConnection(db => db.Query<T>(sql, parameters).FirstOrDefault());
        }

        public static List<T> QueryList<T>(string sql, object? parameters = null)
        {
            return UseConnection(db => db.Query<T>(sql, parameters)).ToList();
        }

        public static string BuildQuerySelectAll(string tablename, object? parameters = null)
        {
             var sql = $"SELECT {tablename}.* FROM {tablename}\n";

            //sql = AddJoins()

            if (parameters == null) return sql;

            sql += AddWhere(tablename, parameters);

            return sql;
        }

        private static string AddJoins()
        {
            return ""; //TODO: Create enitity relations for joins
        }

        private static string AddWhere(string tablename, object parameters)
        {
            IEnumerable<string> propertyNames;

            if (parameters is System.Collections.IDictionary dict)
            {
                propertyNames = dict.Keys.Cast<string>();
            }
            else
            {
                propertyNames = parameters.GetType().GetProperties().Select(p => p.Name);
            }

            if (!propertyNames.Any()) return string.Empty;

            var conditions = propertyNames.Select(name => $"{tablename}.{name} = @{name}");
            return "WHERE " + string.Join(" AND ", conditions);
        }
    }
}
