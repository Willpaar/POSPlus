using Common.Abstract;
using Common.DBEntities.DataRowEntities;
using Common.Helpers;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Common.DBEntities
{
    public class Menu : MenuDataRow
    {

        public override string Name { 
            get => base.Name; 
            set => base.Name = value; 
        }
        public override bool Active { 
            get => base.Active; 
            set => base.Active = value; 
        }

        public static Menu GetActiveMenu()
        {
            var parameters = new Dictionary<string, object>
            {
                { nameof(Active), 1 }
            };

            return SQLHelper.QueryFirst<Menu>(parameters);
        }

    }
}