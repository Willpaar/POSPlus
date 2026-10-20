using Common.Abstract;
using Common.Constants;
using Common.Helpers;

namespace Common.Persistance
{
    public class PersistanceManager
    {
        private List<Entity> Entities { get; set; } = new List<Entity>();
        public T? FindLocalById<T>(Guid id) where T : Entity
        {
            var matches = Entities.OfType<T>().Where(e => e.Id == id).ToList();

            if (matches.Count > 1)
            {
                //help me detect if we run into an issue of multiple of the same object loaded
                throw new InvalidOperationException($"CRITICAL: Identity Map corruption! Multiple instances of {typeof(T).Name} found with ID {id}.");
            }

            return matches.FirstOrDefault();
        }

        public List<T> FindAllLocalByParams<T>(Dictionary<string, object?>? parameters) where T : Entity
        {
            if (parameters == null || !parameters.Any())
            {
                return Entities.OfType<T>().ToList();
            }

            return Entities.OfType<T>().Where(entity =>
            {
                var entityType = entity.GetType();
                foreach (var kvp in parameters)
                {
                    var prop = entityType.GetProperty(kvp.Key);
                    if (prop == null) return false;

                    var entityVal = prop.GetValue(entity);
                    if (!Equals(entityVal, kvp.Value)) return false;
                }
                return true;
            }).ToList();
        }

        public void InsertToEntities(Entity incoming)
        {
            var existing = FindLocalById<Entity>(incoming.Id);

            if (existing == null)
            {
                Entities.Add(incoming);
            }
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