using System.Collections.Generic;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Utils
{
    public static class ArrayExtensions
    {
        public static T[] Add<T>(this T[] that, T item)
        {
            var list = new List<T>(that) {item};
            return list.ToArray();
        }
    }
}