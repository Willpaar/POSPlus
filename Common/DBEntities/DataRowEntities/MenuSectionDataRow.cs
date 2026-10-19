using System;
using System.Collections.Generic;
using System.Linq;
using Common.Abstract;
namespace Common.DBEntities.DataRowEntities
{

    /*
    This File Is auto-generated from GenerateFromDB.ps1
    It is created to mimic the DB structure exactly
    DO NOT MAKE ANY CHANGES TO THIS FILE
    */

    public class MenuSectionDataRow : Entity
    {
        private string _name;
        public virtual string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        private Guid _menuId;
        public virtual Guid MenuId
        {
            get => _menuId;
            set => SetField(ref _menuId, value);
        }

        private Menu _Menu;
        public virtual Menu Menu => _Menu ??= GetFKEntity<Menu>(MenuId);
        private List<MenuItem> _MenuItems;
        public virtual List<MenuItem> MenuItems => _MenuItems ??= GetLinkedEntities<MenuItem>(nameof(MenuItem), Id);

        public MenuSectionDataRow()
        {
        }
    }
}

