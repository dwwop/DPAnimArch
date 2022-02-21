using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AnimArch.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> IfMoreThan<T>(this IEnumerable<T> enumerator, Action<int> action, int threshold = 1)
        {
            int count = enumerator.ToList().Count;
            if (count > threshold)
            {
                action(count);
            }

            foreach (T item in enumerator)
            {
                yield return item;
            }
        }
    }
}
