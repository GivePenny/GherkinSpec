﻿using System.Collections.Generic;
using System.Linq;

namespace GherkinSpec.Model.UnitTests
{
    static class IEnumerableExtensions
    {
        public static T Second<T>(this IEnumerable<T> items)
            => items.Skip(1).First();

        public static T Third<T>(this IEnumerable<T> items)
            => items.Skip(2).First();

        public static T Fourth<T>(this IEnumerable<T> items)
            => items.Skip(3).First();

        public static T Fifth<T>(this IEnumerable<T> items)
            => items.Skip(4).First();
    }
}
