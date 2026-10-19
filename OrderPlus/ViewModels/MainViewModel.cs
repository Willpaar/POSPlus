using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Persistance;
using Menu = Common.DBEntities.Menu;


namespace OrderPlus.ViewModels
{
    public class MainViewModel : BaseViewModel
    {

        public Menu ActiveMenu { get; set; }

        public override void InitializeObjects()
        {
            ActiveMenu = Menu.GetActiveMenu();

            ActiveMenu.Name = "Test";

        }
    }
}
