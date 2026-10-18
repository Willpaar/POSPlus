using System;
using System.Collections.Generic;
using System.Linq;
using Common.Abstract;
namespace Common.DBEntities.DataRowEntities

{
    public class MenuSectionDataRow : Entity
    {
        public virtual string Name { get; set; }
        public virtual Guid MenuId { get; set; }
        public virtual Menu Menu => (Menu)GetFKEntity(nameof(Menu), MenuId);
        public virtual List<MenuItem> MenuItems => GetLinkedEntities<MenuItem>(nameof(MenuItem), Id).Cast<MenuItem>().ToList();

        public MenuSectionDataRow()
        {
        }
    }
}

