using System.Collections.Generic;
using System.Linq;

namespace HiddenSwitch
{
    public static class RandomExtensions
    {
        public static void Shuffle<T>(this IList<T> array)
        {
            var n = array.Count;
            for (var i = 0; i < n; i++)
            {
                // Use Next on random instance with an argument.
                // ... The argument is an exclusive bound.
                //     So we will not go past the end of the array.
                var r = i + UnityEngine.Random.Range(0, n - i);
                var t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }

        /// <summary>
        /// Shuffle compatible with LINQ. <b>Materializes the array</b>.
        /// </summary>
        /// <param name="array"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> Shuffled<T>(this IEnumerable<T> array)
        {
            var materialized = array.ToArray();
            materialized.Shuffle();
            return materialized;
        }
    }
}