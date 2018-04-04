using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static T RandomFromSource<T>(IEnumerable<T> source)
        {
            return source.Random();
        }

        public static IEnumerable<T> ShuffleFromSource<T>(IEnumerable<T> source)
        {
            return source.Shuffle();
        }

        public static T Random<T>(this IEnumerable<T> source)
        {
            return source.Random(1).Single();
        }

        public static IEnumerable<T> Random<T>(this IEnumerable<T> source, int count)
        {
            return source.Shuffle().Take(count);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
        }
    }
}
