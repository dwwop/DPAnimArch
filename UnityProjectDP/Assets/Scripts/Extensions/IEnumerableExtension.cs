using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AnimArch.Extensions
{
    public static class IEnumerableExtension
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

        public static Treturned FirstOrCustomDefault<Tcollection, Treturned>(this IEnumerable<Tcollection> enumerator, Func<Tcollection, Treturned> accessor, Treturned customDefaultValue)
        {
            Tcollection originalFirstOrDefaultValue = enumerator.FirstOrDefault();
            bool valueIsDefault = Comparer<Tcollection>.Default.Compare(originalFirstOrDefaultValue, default) == 0;
            return valueIsDefault ? customDefaultValue : accessor(originalFirstOrDefaultValue);
        }

        public static void ForEach<T>(this IEnumerable<T> enumerator, Action<T> action)
        {
            foreach (T item in enumerator)
            {
                action(item);
            }
        }
    }
}
