using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using SEEDONE.SERVICE.Attributes;

namespace SEEDONE.SERVICE.Helpers
{
    public static class ObjectExtention
    {
        public static string ToJsonString(this object obj, Formatting format = Formatting.None) => JsonConvert.SerializeObject(obj, format);

        public static IEnumerable<Type> GetGenericIEnumerables(this object o)
        {
            return o.GetType()
                    .GetInterfaces()
                    .Where(t => t.IsGenericType
                        && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .Select(t => t.GetGenericArguments()[0]);
        }

        public static string GetDisplayName(this PropertyInfo prop)
        {
            if (prop == null)
            {
                return "";
            }
            var propDisplay = prop.GetCustomAttribute<ColumnNameAttribute>();
            if (propDisplay == null || propDisplay.ColumnName == null)
            {
                return prop.Name;
            }
            return propDisplay.ColumnName;
        }

    
    }
}
