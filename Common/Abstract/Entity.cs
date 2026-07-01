

namespace Common.Abstract
{
    public abstract class Entity
    {
        public Guid Id { get; set; }

      

        public static List<Entity> GetAll()
        {
            throw new NotImplementedException();
        }

        public abstract void Add(Entity entity);

        public abstract void Remove(Entity entity);

        public abstract void Update(Entity entity);

    }
}