using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    public static class CtrlFlowExt
    {
        public static void ForEach(this int n, Action<int> body)
        {
            for (var i = 0; i < n; i++)
            {
                body(i);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> seq, Action<T> body)
        {
            foreach (var s in seq.ToList())
            {
                body(s);
            }
        }
    }
}