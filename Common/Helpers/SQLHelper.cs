using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Windows.Controls;
using Common.Abstract;
using Common.Constants;
using Common.Persistance;
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

        private static TResult UseConnection<TResult>(Func<SqlConnection, TResult> getData)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                return getData(connection);
            }
        }
        internal static void TryPassThruQuery(string sql)
        {
            UseConnection(db =>
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        db.Execute(sql, transaction: transaction);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
                return true;
            });
        }


        public static T? QueryFirst<T>(object? parameters = null) where T : Entity
        {
            return GetAndRunQuery<T>(parameters).FirstOrDefault();
        }

        public static List<T> QueryList<T>(object? parameters = null) where T : Entity
        {
            return GetAndRunQuery<T>(parameters).ToList();
        }

        public static List<T> QueryRaw<T>(string sql, object? parameters = null) where T : Entity
        {
            return Initialize(UseConnection(db => db.Query<T>(sql, parameters))).ToList();
        }

        private static IEnumerable<T> GetAndRunQuery<T>(object? parameters = null) where T : Entity
        {
            var sql = BuildQuerySelectAll(EntityConsts.CorrectTableName<T>(), parameters);
            return Initialize(UseConnection(db => db.Query<T>(sql, parameters)));
        }
        private static IEnumerable<T> Initialize<T>(IEnumerable<T> entities) where T : Entity
        {
            foreach(var e in entities)
            {
                e.RowState = RowStateType.Unchanged;
                e.Initializaed = true;
                Context.PM.InsertToEntities(e);
            }
            return entities;
        }

        private static string BuildQuerySelectAll(string tablename, object? parameters = null)
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

        public static string InsertSQL<T>(T entity) where T : Entity
        {
            var tableName = entity.TableName;
            if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();

            var propNames = entity.Properties.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            propNames.Insert(0, nameof(Entity.Id));

            var type = entity.GetType();
            var values = propNames.Select(propName =>
            {
                var val = type.GetProperty(propName)?.GetValue(entity);
                return FormatSqlValue(val);
            });

            var columns = string.Join(", ", propNames);
            var valueString = string.Join(", ", values);

            return $"INSERT INTO {tableName} ({columns}) VALUES ({valueString})";
        }

        public static string UpdateSQL<T>(T entity) where T : Entity
        {
            var tableName = entity.TableName;
            var propNames = entity.Properties.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

            var type = entity.GetType();
            var setClauses = propNames.Select(propName =>
            {
                var val = type.GetProperty(propName)?.GetValue(entity);
                return $"{propName} = {FormatSqlValue(val)}";
            });

            return $"UPDATE {tableName} SET {string.Join(", ", setClauses)} WHERE Id = '{entity.Id}'";
        }

        public static string DeleteSQL<T>(T entity) where T : Entity
        {
            return $"DELETE FROM {entity.TableName} WHERE Id = '{entity.Id}'";
        }

        private static string FormatSqlValue(object? val)
        {
            if (val == null || val is DBNull) return "NULL";
            if (val is string || val is Guid || val is DateTime) return $"'{val}'";
            if (val is bool b) return b ? "1" : "0";
            return val.ToString() ?? "NULL";
        }
    }
}