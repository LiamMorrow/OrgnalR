using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OrgnalR.Core.Data
{
    public class SingletonList<T> : IReadOnlyList<T>
    {
        private readonly T value;
        public SingletonList(T value)
        {
            this.value = value;
        }

        public T this[int index] => index == 0 ? value : throw new ArgumentOutOfRangeException("List contains 1 element, provided " + index);

        public int Count => 1;

        public IEnumerator<T> GetEnumerator()
        {
            return new SingletonEnumerator<T>(value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}