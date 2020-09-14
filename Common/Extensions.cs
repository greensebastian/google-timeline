using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public static class Extensions
    {
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> enumerable) => enumerable.Where(value => value != null);
    }
}
