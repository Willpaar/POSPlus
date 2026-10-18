

using Common.Constants;

namespace Common.Abstract
{
    public abstract class Entity
    {
        public Guid Id { get; set; }

        public RowStateType RowState { get; internal set; }

        public string TableName { get; set; }

        public string Properties { get; set; }

        public Entity GetFKEntity()
        {
            return null;
        }
     
    }
}