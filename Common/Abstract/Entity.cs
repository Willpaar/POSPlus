using Common.Helpers;
using Common.Constants;
using System;
using Common.Persistance;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Common.Abstract
{
    public abstract class Entity : INotifyPropertyChanged
    {
        public Guid Id { get; set; }

        private RowStateType _rowState = RowStateType.Unchanged;
        public RowStateType RowState
        {
            get => _rowState;
            internal set => _rowState = value;
        }

        internal bool Initializaed { get; set; } = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            field = value;
            OnPropertyChanged(propertyName);

            if (!Initializaed) return true;

            Context.PM.ModifyEntity(this);

            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual string TableName => EntityConsts.CorrectTableName(GetType());
        private Type GetDataRowType()
        {
            var t = GetType();
            while (t != null && t != typeof(Entity))
            {
                if (t.Name.EndsWith(EntityConsts.DataRow)) return t;
                t = t.BaseType;
            }

            throw new InvalidOperationException($"Entity type '{GetType().Name}' must inherit from a generated *DataRow class to be mapped to the database.");
        }

        public virtual string Properties => string.Join(", ",
            GetDataRowType().GetProperties()
                .Where(p => p.Name != nameof(Id)
                           && p.Name != nameof(RowState)
                           && p.Name != nameof(TableName)
                           && p.Name != nameof(Properties)
                           && !typeof(Entity).IsAssignableFrom(p.PropertyType)
                           && (p.PropertyType == typeof(string) || !typeof(System.Collections.IEnumerable).IsAssignableFrom(p.PropertyType)))
                .Select(p => p.Name)
        );

        protected T? GetFKEntity<T>(Guid? fkId) where T : Entity
        {
            if (!fkId.HasValue || fkId.Value == Guid.Empty) return null;
            return SQLHelper.QueryFirst<T>(new Dictionary<string, object?> { { nameof(Id), fkId.Value } });
        }

        protected List<T> GetFKEntities<T>(string foreignKeyColumnName, Guid parentId) where T : Entity
        {
            var parameters = new Dictionary<string, object>
            {
                { foreignKeyColumnName, parentId }
            };
            return SQLHelper.QueryList<T>(parameters);
        }

        protected List<T> GetLinkedEntities<T>(string linkTableName, Guid entityId) where T : Entity
        {
            var targetTable = EntityConsts.CorrectTableName<T>();
            linkTableName = EntityConsts.CorrectLinkTableName(linkTableName);

            var sql = $@"SELECT {targetTable}.* FROM {targetTable} 
                        INNER JOIN {linkTableName} ON {targetTable}.Id = {linkTableName}.{targetTable}Id 
                        WHERE {linkTableName}.{TableName}Id = @Id";

            return SQLHelper.QueryRaw<T>(sql, new Dictionary<string, object?> { { nameof(Id), entityId } });
        }

    }
}