using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEEDONE.SERVICE.Attributes
{
    public class ColumnNameAttribute : Attribute
    {
        public string ColumnName { set; get; }
        public ColumnNameAttribute(string columnName)
        { this.ColumnName = columnName; }
    }
}
