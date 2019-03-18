using System;
using System.Collections.Generic;

namespace Neurotic
{
    public static class Extensions
    {
        /// <summary>
        /// Fills what collection with result count times.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="what"></param>
        /// <param name="with"></param>
        /// <param name="count"></param>
        public static void Fill<T>(this ICollection<T> what, Func<T> with, int count)
        {
            if (count <= 0) return;
            if (what == null) throw new ArgumentNullException(nameof(what));
            var i = -1;
            do
            {
                i++;
                what.Add(with());
            } while (i < count);
        }
        /// <summary>
        /// Fills what collection with result count times.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="what"></param>
        /// <param name="with"></param>
        /// <param name="count"></param>
        public static void Fill<T>(this ICollection<T> what, Func<int,T> with, int count)
        {
            if (count <= 0) return;
            if (what == null) throw new ArgumentNullException(nameof(what));
            var i = -1;
            do
            {
                i++;
                what.Add(with(i));
            } while (i < count);
        }
        /// <summary>
        /// Fills what collection with result count times, then perform an action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="what"></param>
        /// <param name="with"></param>
        /// <param name="count"></param>
        /// /// <param name="then"></param>
        public static void Fill<T>(this ICollection<T> what, Func<T> with, int count, Action<T> then)
        {
            if (count <= 0) return;
            if (what == null) throw new ArgumentNullException(nameof(what));
            var i = -1;
            do
            {
                i++;
                var r = with();
                then(r);
                what.Add(r);
            } while (i < count);
        }
        /// <summary>
        /// Fills what collection with result count times, then perform an action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="what"></param>
        /// <param name="with"></param>
        /// <param name="count"></param>
        /// /// <param name="then"></param>
        public static void Fill<T>(this ICollection<T> what, Func<int, T> with, int count, Action<int,T> then)
        {
            if (count <= 0) return;
            if (what == null) throw new ArgumentNullException(nameof(what));
            var i = -1;
            do
            {
                i++;
                var r = with(i);
                then(i,r);
                what.Add(r);
            } while (i < count);
        }
    }
}