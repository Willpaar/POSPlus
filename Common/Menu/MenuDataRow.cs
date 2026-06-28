using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using Dapper;
using Common.Abstract;

namespace Common
{
    public class MenuDataRow : Entity
    {
        public string Name { get; set; }
        public bool Active { get; set; }

        public MenuDataRow()
        {
        }

        public override void Load()
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                string sql = "SELECT TOP 1 Id, Name, Active FROM Menu";

                var data = connection.Query<MenuDataRow>(sql).FirstOrDefault();

                if (data != null)
                {
                    this.Id = data.Id;
                    this.Name = data.Name;
                    this.Active = data.Active;
                }
            }
        }

        public override void Save()
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                if (this.Id == Guid.Empty)
                {
                    this.Id = Guid.NewGuid();
                }

                string sql = @"
                    MERGE Menu AS Target
                    USING (SELECT @Id AS Id) AS Source
                    ON (Target.Id = Source.Id)
                    WHEN MATCHED THEN
                        UPDATE SET Name = @Name, Active = @Active
                    WHEN NOT MATCHED THEN
                        INSERT (Id, Name, Active) VALUES (@Id, @Name, @Active);";

                connection.Execute(sql, new { Id = this.Id, Name = this.Name, Active = this.Active });
            }
        }
    }
}