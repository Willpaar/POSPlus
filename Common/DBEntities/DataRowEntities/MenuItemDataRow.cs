using System;
using System.Collections.Generic;
using System.Linq;
using Common.Abstract;
namespace Common.DBEntities.DataRowEntities

{
    public class MenuItemDataRow : Entity
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string Section { get; set; }
        public virtual decimal Price { get; set; }
        public virtual List<MenuSection> MenuSections => GetLinkedEntities<MenuSection>(nameof(MenuItem), Id).Cast<MenuSection>().ToList();

        public MenuItemDataRow()
        {
        }
    }
}

