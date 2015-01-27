using System;
using SbManager.Models.ViewModels;

namespace SbManager.Extensions
{
    public static class ValueExtensions
    {
        public static bool HasValue(this string val)
        {
            return !string.IsNullOrEmpty(val);
        }

        public static bool HasValue(this DateTime val)
        {
            return val != DateTime.MinValue;
        }

        public static bool HasValue(this DateTime? val)
        {
            return HasValue(val.GetValueOrDefault());
        }

        public static bool HasValue(this int val)
        {
            return val != 0;
        }

        public static bool HasValue(this int? val)
        {
            return HasValue(val.GetValueOrDefault());
        }

        public static bool HasValue(this TimeSpan val)
        {
            return val != TimeSpan.MinValue && !val.TotalMilliseconds.Equals(0);
        }
    }
}
