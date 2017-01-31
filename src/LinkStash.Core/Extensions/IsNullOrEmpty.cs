﻿namespace LinkStash.Core
{
    using System.Collections.Generic;
    using System.Linq;

    public static partial class Extensions
    {
        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source)
        {
            return source == null || !source.Any();
        }

        public static bool IsNullOrEmpty<TSource>(this ICollection<TSource> source)
        {
            return source == null || !source.Any();
        }

        public static bool IsNullOrEmpty<TSource>(this IReadOnlyCollection<TSource> source)
        {
            return source == null || !source.Any();
        }
    }
}