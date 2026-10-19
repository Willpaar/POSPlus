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

    public class MenuItemDataRow : Entity
    {
        private string _name;
        public virtual string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        private string _description;
        public virtual string Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        private string _section;
        public virtual string Section
        {
            get => _section;
            set => SetField(ref _section, value);
        }

        private decimal _price;
        public virtual decimal Price
        {
            get => _price;
            set => SetField(ref _price, value);
        }

        private List<MenuSection> _MenuSections;
        public virtual List<MenuSection> MenuSections => _MenuSections ??= GetLinkedEntities<MenuSection>(nameof(MenuItem), Id);

        public MenuItemDataRow()
        {
        }
    }
}

