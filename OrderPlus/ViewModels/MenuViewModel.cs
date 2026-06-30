using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menu = Common.Menu.Menu;

namespace OrderPlus.ViewModels
{
    public class MenuViewModel : BaseViewModel
    {
        public Menu ActiveMenu { get; set; }

        public override void InitializeObjects()
        {
            if (ActiveMenu == null) ActiveMenu = Menu.GetActiveMenu();
        }
    }
}
