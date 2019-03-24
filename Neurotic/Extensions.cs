using System;
using System.Collections.Generic;
using System.Linq;

namespace Neurotic
{
    public static class Extensions
    {
        public static T[] OffsetCenteredWrappedSubset<T>(this IEnumerable<T> arr, int offset, int count)
        {
            
            if (arr.Count() < 1 || count < 1) return new T[0];
            if (count % 2 == 0) count += 1;
            if (offset < 0 || offset > arr.Count() - 1) throw new ArgumentOutOfRangeException(nameof(offset));
            
            List<T> temp = new List<T>();
            if (offset == 0)
            {
                temp.AddRange(arr.Skip(arr.Count() - (count / 2)));
                temp.AddRange(arr.Take(1 + (count / 2)));
            } else if (offset == arr.Count() - 1)
            {
                temp.AddRange(arr.Skip(offset - (count / 2)));
                temp.AddRange(arr.Take((count / 2)));
            }
            else if (offset - (count / 2) < 0)
            {
                
                temp.AddRange(arr.Skip( arr.Count() + (offset - (count / 2))  ));
                temp.AddRange(arr.Take(offset + (1 + count / 2)));
            }
            else if (offset + (count / 2) > arr.Count() - 1)
            {
                temp.AddRange(arr.Skip(offset - 1));
                temp.AddRange(arr.Take(count / 2));
            }
            else
            {
                temp.AddRange(arr.Skip(offset - (count / 2)).Take(count));
            }
            return temp.ToArray();
        }

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
            var i = 0;
            while(i < count)
            {
                i++;
                what.Add(with());
            }
        }
        /// <summary>
        /// Fills what collection with result count times.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="what"></param>
        /// <param name="with"></param>
        /// <param name="count"></param>
        public static void Fill<T>(this ICollection<T> what, Func<int, T> with, int count)
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
        public static void Fill<T>(this ICollection<T> what, Func<int, T> with, int count, Action<int, T> then)
        {
            if (count <= 0) return;
            if (what == null) throw new ArgumentNullException(nameof(what));
            var i = -1;
            do
            {
                i++;
                var r = with(i);
                then(i, r);
                what.Add(r);
            } while (i < count);
        }
    }
}