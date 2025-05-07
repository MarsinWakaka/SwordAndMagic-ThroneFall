using System.Collections.Generic;

namespace MyFramework.Utilities.Extensions
{
    public static class CollectionExtension
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }
    }
}