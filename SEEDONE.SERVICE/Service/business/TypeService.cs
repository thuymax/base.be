using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SEEDONE.SERVICE.Attributes;
using SEEDONE.SERVICE.Interfaces.Service.Business;

namespace SEEDONE.SERVICE.Service.Business
{
    public class TypeService : ITypeService
    {
        public PropertyInfo GetKeyField(Type type)
        {
            return GetKeyFields(type).FirstOrDefault();
        }

        public List<PropertyInfo> GetKeyFields(Type type)
        {
            var keys = this.GetPropertys<KeyAttribute>(type);
            return keys.Select(n => n.Key).ToList();
        }

        public string GetMasterTableName(Type type)
        {
            var attr = type.GetCustomAttribute<MasterAttribute>();
            if (attr == null)
            {
                return null;
            }
            return attr.Name;
        }

        public Dictionary<PropertyInfo, TAttribute> GetPropertys<TAttribute>(Type type) where TAttribute : Attribute
        {
            if (type == null)
            {
                return null;
            }

            var result = new Dictionary<PropertyInfo, TAttribute>();
            var prs = type.GetProperties();
            foreach (var pr in prs)
            {
                var attr = pr.GetCustomAttribute<TAttribute>(true);
                if (attr != null)
                {
                    result.Add(pr, attr);
                }
            }

            return result;
        }

        public List<string> GetTableColumns(Type type)
        {
            if (type == null)
            {
                return null;
            }
            var fields = new List<string>();
            var prs = type.GetProperties();
            foreach (var item in prs)
            {
                if (item.GetCustomAttribute<IgnoreUpdateAttribute>() == null)
                {
                    fields.Add(item.Name);
                }
            }
            return fields;
        }

        public string GetTableName(Type type)
        {
            var attr = type.GetCustomAttribute<TableAttribute>();
            if (attr == null)
            {
                return null;
            }
            return attr.Table;
        }

        public Type GetTypeInList(Type listType)
        {
            if (this.IsList(listType))
            {
                Type type = listType.GetGenericArguments()[0];
                return type;
            }
            return null;
        }

        public bool IsList(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }
    }
}
