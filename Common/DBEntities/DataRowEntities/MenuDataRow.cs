using System;
using System.Collections.Generic;
using System.Linq;
using Common.Abstract;
namespace Common.DBEntities.DataRowEntities

{
    public class MenuDataRow : Entity
    {
        public virtual string Name { get; set; }
        public virtual bool Active { get; set; }
        public virtual List<MenuSection> MenuSections => GetFKEntities(nameof(MenuSection), Id).Cast<MenuSection>().ToList();

        public MenuDataRow()
        {
        }
    }
}

