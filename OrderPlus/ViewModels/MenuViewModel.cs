using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Menu;
using Menu = Common.Menu.Menu;

namespace OrderPlus.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        public Menu ActiveMenu { get; set; }

        public List<MenuSection> MenuSections { get; set; }

        public override void InitializeObjects()
        {
            //TODO:figure out loading system

            ActiveMenu = Menu.GetActiveMenu();

            MenuSections = MenuSection.GetMenuSections(ActiveMenu);
        }
    }
}
