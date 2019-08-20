using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OrgnalR.Core.Data
{
    public class EmptySet<T> : ISet<T>
    {
        private EmptySet() { }
        public static readonly EmptySet<T> Instance = new EmptySet<T>();
        public int Count => 0;

        public bool IsReadOnly => true;

        public bool Add(T item)
        {
            throw new System.NotSupportedException();
        }

        public void Clear()
        {
            throw new System.NotSupportedException();
        }

        public bool Contains(T item) => false;

        public void CopyTo(T[] array, int arrayIndex)
        {
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            throw new System.NotSupportedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return EmptyEnumerator<T>.Instance;
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            throw new System.NotSupportedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other) => true;
        public bool IsProperSupersetOf(IEnumerable<T> other) => !other.Any();

        public bool IsSubsetOf(IEnumerable<T> other) => true;

        public bool IsSupersetOf(IEnumerable<T> other) => !other.Any();

        public bool Overlaps(IEnumerable<T> other) => false;

        public bool Remove(T item)
        {
            throw new System.NotSupportedException();
        }

        public bool SetEquals(IEnumerable<T> other) => !other.Any();

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new System.NotSupportedException();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            throw new System.NotSupportedException();
        }

        void ICollection<T>.Add(T item)
        {
            throw new System.NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}