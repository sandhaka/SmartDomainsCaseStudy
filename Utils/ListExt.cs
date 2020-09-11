using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    public static class ListExt
    {
        public static string AsString(this IEnumerable<char> list)
        {
            return new string(list.ToArray());
        }

        public static string AsString(this IEnumerable<string> list)
        {
            return list.Aggregate((_1, _2) => $"{_1}{_2}");
        }
        
        public static IEnumerable<TResult> FilterCast<TResult>(this IEnumerable source)
        {
            var s2 = source as IEnumerable<object>;
            return s2!.Where(s => s.GetType() == typeof(TResult))
                .Cast<TResult>();
        }
        
        public static void ForEach<T>(this IEnumerable<T> seq, Action<T, T> action)
        {
            var list = seq.ToList();
            
            if (action == null)
            {
                throw new NullReferenceException(nameof(action));
            }

            var size = list.Count;

            for (var i = 0; i < size; i++)
            {
                action!(i > 0 ? list[i-1] : default, list[i]);
            }
        }
    }
}