using Common.Abstract;
using Common.Helpers;

namespace Common.Menu
{
    public class MenuSection : Entity
    {
        public string Name { get; set; }

        public Guid MenuId { get; set; }

        public MenuSection()
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