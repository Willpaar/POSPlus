using Common.Abstract;
using Common.DBEntities.DataRowEntities;
using Common.Helpers;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Common.DBEntities
{
    public class Menu : MenuDataRow
    {

        public override string Name { get; set; }
        public override bool Active { get; set; }

        public static Menu GetActiveMenu()
        {
            object parameters = new Dictionary<string, object>
            {
                { nameof(Active), 1 }
            };

            string sql = SQLHelper.BuildQuerySelectAll(nameof(Menu), parameters);

            return SQLHelper.QueryFirst<Menu>(sql, parameters);
        }

    }
}