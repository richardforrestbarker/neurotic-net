using System;
using System.Collections.Generic;

namespace Tests
{
    public static class Extensions
    {
        public static void Fill<T>(this ICollection<T> what, Func<T> with, uint count)
        {
            if (what == null) throw new ArgumentNullException(nameof(what));
            var i = -1;
            do
            {
                i++;
                what.Add(with());
            } while (i < count);
        }
    }
}