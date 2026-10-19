using Common.Abstract;
using Common.Constants;
using Common.Helpers;

namespace Common.Persistance
{
    public class PersistanceManager
    {
        internal List<Entity> Entities { get; set; } = new List<Entity>();

        public void InsertToEntities(Entity e)
        {
            if (!Entities.Contains(e)) Entities.Add(e);
        }

        public void RemoveFromEntities(Entity e) 
        {
            if (Entities.Contains(e)) Entities.Remove(e); 
        }
        public void AddEntity(Entity e)
        {
            e.RowState = RowStateType.Added;
            e.Initializaed = true;
            InsertToEntities(e);
        }

        public void ModifyEntity(Entity e)
        {
            if (e.RowState == RowStateType.Unchanged)
            {
                e.RowState = RowStateType.Modified;
            }
            InsertToEntities(e);
        }

        public void DeleteEntity(Entity e)
        {
            e.RowState = RowStateType.Deleted;
            InsertToEntities(e);
        }

        public void SaveAllChanges()
        {
            var sql = string.Empty;
            foreach (var e in Entities)
            {
                sql += GetSQL(e) + ";\n";
            }

            if (string.IsNullOrWhiteSpace(sql)) return;

            try
            {
                SQLHelper.TryPassThruQuery(sql);

                foreach (var e in Entities)
                {
                    e.RowState = RowStateType.Unchanged;
                }
                Entities.Clear();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetSQL(Entity e)
        {
            switch (e.RowState)
            {
                case RowStateType.Added:
                    return SQLHelper.InsertSQL((dynamic)e);
                case RowStateType.Modified:
                    return SQLHelper.UpdateSQL((dynamic)e);
                case RowStateType.Deleted:
                    return SQLHelper.DeleteSQL((dynamic)e);
                default:
                    return string.Empty;
            }
        }
    }
}