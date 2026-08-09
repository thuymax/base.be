using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEEDONE.SERVICE.Attributes
{
    public class DetailAttribute : Attribute, IMasterRefAttribute
    {
        public string MasterKeyField { get; set; }
        public Type Type { get; set; }
        public int Index { get; set; }

        public DetailAttribute(string masterKeyField, Type type)
        {
            MasterKeyField = masterKeyField;
            Type = type;
        }
        public DetailAttribute(string masterKeyField, Type type, int index)
        {
            MasterKeyField = masterKeyField;
            Type = type;
            Index = index;
        }
    }
}
