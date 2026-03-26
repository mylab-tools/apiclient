using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace MyLab.ApiClient
{
    static class ObjectToStringConverter
    {
        public static string ToString(object target)
        {
            if (target == null) return string.Empty;

            if (target is DateTime dt)
            {
                return dt.ToString("o", CultureInfo.InvariantCulture);
            }

            if (target is Guid guid)
            {
                return guid.ToString("N");
            }

            if(target.GetType().IsEnum)
            {
                return FromEnum(target);
            }

            return target.ToString();
        }

        static string FromEnum(object target)
        {
            // Получаем тип перечисления
            var enumType = target.GetType();
            // Проверяем, что переданный тип является перечислением
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Тип должен быть перечислением");
            }
            // Получаем поле, соответствующее значению
            var field = enumType.GetFields(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(f => Equals(target, f.GetValue(null)));
            if (field == null)
            {
                return null; // Если значение не найдено
            }
            // Проверяем наличие атрибута EnumMemberAttribute
            var enumMemberAttribute = field.GetCustomAttribute<EnumMemberAttribute>();
            if (enumMemberAttribute != null)
            {
                return enumMemberAttribute.Value; // Возвращаем значение из EnumMemberAttribute
            }
            // Если атрибут отсутствует, возвращаем имя поля
            return field.Name;
        }
    }
}