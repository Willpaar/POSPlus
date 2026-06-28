using System;
using System.IO;
using Microsoft.Extensions.Configuration;

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
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        protected string GetConnectionString()
        {
            return _connectionString;
        }

        public abstract void Save();
        public abstract void Load();
    }
}