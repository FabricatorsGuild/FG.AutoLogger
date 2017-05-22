using System;
using System.Text;

namespace CodeEffect.Diagnostics.EventSourceGenerator.Samples
{
    public static class IntListHelper
    {
        public static string GetSummary(this int[] that)
        {
            var stringBuilder = new StringBuilder();
            var delimiter = "";
            for (var i = 0; i < Math.Min(that.Length, 3); i++)
            {
                stringBuilder.Append($"{delimiter}{i}");
                delimiter = ", ";
            }
            stringBuilder.Append($"... {that.Length}");

            return stringBuilder.ToString();
        }
    }
}