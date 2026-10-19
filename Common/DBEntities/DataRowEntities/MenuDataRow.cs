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

    public class MenuDataRow : Entity
    {
        private string _name;
        public virtual string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        private bool _active;
        public virtual bool Active
        {
            get => _active;
            set => SetField(ref _active, value);
        }

        private List<MenuSection> _MenuSections;
        public virtual List<MenuSection> MenuSections => _MenuSections ??= GetFKEntities<MenuSection>("MenuId", Id);

        public MenuDataRow()
        {
        }
    }
}

