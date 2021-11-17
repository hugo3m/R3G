using System.Collections;
using System.Collections.Generic;

namespace Recognizer.EvaluationTools
{
    public static class IListExtension
    {
        /// <summary>
        /// Shuffles the element order of the specified list.
        /// </summary>
        public static  List<T> Shuffle<T>(this IList<T> ts)
        {
            List<T> l = new List<T>(ts);
            var count = l.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = l[i];
                l[i] = l[r];
                l[r] = tmp;
            }

            return l;
        }
    }
}