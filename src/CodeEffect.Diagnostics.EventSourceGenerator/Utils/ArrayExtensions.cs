using System;
using System.Collections.Generic;
using System.Linq;

namespace FG.Diagnostics.AutoLogger.Generator.Utils
{
    public static class ArrayExtensions
    {
        public static T[] Add<T>(this T[] that, T item)
        {
            var list = that == null ? new List<T> {item} : new List<T>(that) {item};
            return list.ToArray();
        }

	    public static IEnumerable<T> SafeLinq<T>(this IEnumerable<T> that, T defaultValue)
	    {
		    if (!that.Any()) return new T[]{defaultValue};

		    return that;
	    }

	    public static TResult SafeLinq<T, TResult>(this IEnumerable<T> that, Func<IEnumerable<T>, TResult> linq, TResult defaultValue)
	    {
		    if (that == null || !that.Any()) return defaultValue;
		    return linq(that);
	    }
	}    
}