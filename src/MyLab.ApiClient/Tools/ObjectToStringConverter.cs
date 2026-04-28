using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

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
                return dt.ToString("O");
            }

            if (target is DateTimeOffset dto)
            {
                return dto.ToString("O");
            }

            if (target is DateOnly date)
            {
                return date.ToString("O");
            }

            if (target is TimeOnly t)
            {
                return t.ToString("O");
            }

            if (target is TimeSpan ts)
            {
                return ts.ToString("c");
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

            if (target is Enum enm)
            {
                return EnumToString(enm);
            }

            return target.ToString() ?? throw new InvalidOperationException();
        }

        static string EnumToString(Enum enm)
        {
            var enType = enm.GetType();
            var fieldInfo = enType.GetField(enm.ToString());
            
            if (fieldInfo == null)
            {
                return enm.ToString();
            }

            var mAttr = fieldInfo.GetCustomAttribute<EnumMemberAttribute>();
            
            return mAttr is { Value: not null } ? mAttr.Value : enm.ToString();
        }
    }
}
