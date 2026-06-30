using Common.Abstract;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Common.Menu {
    public class Menu : Entity
    {

        public string Name { get; set; }
        public bool Active { get; set; }

        public Menu()
        {
        }

        public override void Add(Entity entity)
        {
            throw new NotImplementedException();
        }

        public override void Remove(Entity entity)
        {
            throw new NotImplementedException();
        }

        public override void Update(Entity entity)
        {
            throw new NotImplementedException();
        }

        public static Menu GetActiveMenu()
        {
            return QueryFirst<Menu>("SELECT TOP 1 * FROM Menu WHERE Active = 1");
        }
    }
}