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
    }
}