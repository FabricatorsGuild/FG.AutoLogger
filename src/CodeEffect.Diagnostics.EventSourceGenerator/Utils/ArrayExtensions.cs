using System.Collections.Generic;

namespace FG.Diagnostics.AutoLogger.Generator.Utils
{
    public static class ArrayExtensions
    {
        public static T[] Add<T>(this T[] that, T item)
        {
            var list = that == null ? new List<T> {item} : new List<T>(that) {item};
            return list.ToArray();
        }
    }    
}