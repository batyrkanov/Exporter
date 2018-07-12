using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace Exporter.Tests.Models.Tests.Entities.Tests
{
    public class TestDbSet<T> : IDbSet<T> where T : class
    {
        private List<PropertyInfo> _keys;
        private ObservableCollection<T> _data;
        private IQueryable _query;

        public TestDbSet()
        {
            _keys = typeof(T)
                .GetProperties()
                .Where(p => Attribute.IsDefined(
                    p, typeof(KeyAttribute)) ||
                    "Id".Equals(p.Name, StringComparison.Ordinal))
                .ToList();
            _data = new ObservableCollection<T>();
            _query = _data.AsQueryable();
        }

        public T Find(params object[] keyValues)
        {
            if (keyValues == null)
                throw new ArgumentNullException("keyValues");

            return _data.ElementAtOrDefault((int)keyValues[0]);
        }

        public T Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException("entity");
            
            _data.Add(item);
            return item;
        }

        public T Remove(T item)
        {
            _data.Remove(item);
            return item;
        }

        public T Attach(T item)
        {
            _data.Add(item);
            return item;
        }

        public T Detach(T item)
        {
            _data.Remove(item);
            return item;
        }

        public T Create()
        {
            return Activator.CreateInstance<T>();
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public ObservableCollection<T> Local
        {
            get { return _data; }
        }

        //public IEnumerable<T> AddRange(IEnumerable<T> entities)
        //{
        //    _data.AddRange(entities);
        //    return _data;
        //}

        public IEnumerable<T> RemoveRange(IEnumerable<T> entities)
        {
            for (int i = entities.Count() - 1; i >= 0; i--)
            {
                T entity = entities.ElementAt(i);
                if (_data.Contains(entity))
                {
                    Remove(entity);
                }
            }
            return this;
        }

        Type IQueryable.ElementType
        {
            get { return _query.ElementType; }
        }

        System.Linq.Expressions.Expression IQueryable.Expression
        {
            get { return _query.Expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return _query.Provider; }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _data.GetEnumerator();
        }
    }
}
