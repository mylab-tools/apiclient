using System;
using System.Globalization;

namespace MyLab.ApiClient
{
    static class ObjectToStringConverter
    {
        public static string ToString(object target)
        {
            if (target == null) return string.Empty;

            if (target is DateTime dt)
            {
                return dt.ToString(CultureInfo.InvariantCulture);
            }

            return target.ToString();
        }
    }
}