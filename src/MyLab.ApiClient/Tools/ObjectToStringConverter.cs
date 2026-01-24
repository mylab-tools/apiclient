using System;
using System.Collections;
using System.Globalization;
using System.Linq;

namespace MyLab.ApiClient.Tools
{
    static class ObjectToStringConverter
    {
        public static string ToString(object? target)
        {
            if (target == null) return string.Empty;

            if (target is string str)
            {
                return str;
            }
            
            if (target is DateTime dt)
            {
                return dt.ToString("s");
            }

            if (target is Guid guid)
            {
                return guid.ToString("N");
            }

            if (target is double d)
            {
                return d.ToString(CultureInfo.InvariantCulture);
            }

            if (target is IEnumerable en)
            {
                return string.Join(",", en.Cast<object>().Select(ToString));
            }

            return target.ToString() ?? throw new InvalidOperationException();
        }
    }
}
