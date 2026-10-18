using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Constants;
using static Dapper.SqlMapper;

namespace Common.Abstract
{
    public class PersistanceManager
    {
        public List<Entity> Entities { get; set; }


        public void SaveAllChanges()
        {
            foreach (var e in Entities)
            {
                SaveEntity(e);
            }

        }
        
        public void SaveEntity(Entity e)
        {

            switch (e.RowState)
            {
                case RowStateType.Added:
                    InsertEntity();
                    break;
                case RowStateType.Modified:
                    UpdateEntity();
                    break;
                case RowStateType.Deleted:
                    DeleteEntity();
                    break;
                default: break;
            }

        }

        private void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        private void UpdateEntity()
        {
            throw new NotImplementedException();
        }

        private void InsertEntity()
        {
            throw new NotImplementedException();
        }
    }
}
