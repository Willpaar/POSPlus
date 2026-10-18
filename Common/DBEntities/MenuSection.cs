using Common.Abstract;
using Common.Helpers;

namespace Common.DBEntities
{
    public class MenuSection : Entity
    {
        public string Name { get; set; }

        public Guid MenuId { get; set; }

        public MenuSection()
        {
        }

        public static List<MenuSection> GetMenuSections(Menu Menu)
        {
            object parameters = new Dictionary<string, object>
            {
                { nameof(MenuId), Menu.Id }
            };

            string sql = SQLHelper.BuildQuerySelectAll(nameof(MenuSection), parameters);

            return SQLHelper.QueryList<MenuSection>(sql, parameters);
        }
    }
}