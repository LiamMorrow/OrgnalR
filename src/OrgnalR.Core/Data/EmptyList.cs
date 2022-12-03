using System;
using System.Collections;
using System.Collections.Generic;
using Orleans;

namespace OrgnalR.Core.Data
{
    [GenerateSerializer]
    public class EmptyList<T> : IReadOnlyList<T>
    {
        private EmptyList() { }

        public static readonly EmptyList<T> Instance = new();
        public int Count => 0;

        public T this[int index] => throw new ArgumentOutOfRangeException(nameof(index));

        public IEnumerator<T> GetEnumerator()
        {
            return EmptyEnumerator<T>.Instance;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EmptyEnumerator<T>.Instance;
        }
    }
}
