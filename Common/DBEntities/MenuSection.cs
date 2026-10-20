using Common.Abstract;
using Common.DBEntities.DataRowEntities;
using Common.Helpers;

namespace Common.DBEntities
{
    public class MenuSection : MenuSectionDataRow
    {
        public override string Name { get; set; }

        public override Guid MenuId { get; set; }

        public MenuSection()
        {
        }

        public static List<MenuSection> GetMenuSections(Menu Menu)
        {
            var parameters = new Dictionary<string, object?>
            {
                { nameof(MenuId), Menu.Id }
            };

            return SQLHelper.QueryList<MenuSection>(parameters);
        }
    }
}