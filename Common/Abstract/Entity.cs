using System;
using System.IO;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Common.Abstract
{
    public abstract class Entity
    {
        public Guid Id { get; set; }

        private static readonly string _connectionString;

        static Entity()
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

        protected static T? QueryFirst<T>(string sql, object? parameters = null)
        {
            return UseConnection(db => db.Query<T>(sql, parameters).FirstOrDefault());
        }

        protected static List<T> QueryList<T>(string sql, object? parameters = null)
        {
            return UseConnection(db => db.Query<T>(sql, parameters)).ToList();
        }

        public static List<Entity> GetAll()
        {
            throw new NotImplementedException();
        }

        public abstract void Add(Entity entity);

        public abstract void Remove(Entity entity);

        public abstract void Update(Entity entity);

    }
}