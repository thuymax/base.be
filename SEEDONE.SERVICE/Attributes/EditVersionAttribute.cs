using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEEDONE.SERVICE.Attributes
{
    /// <summary>
    /// Attribute đánh dấu dữ liệu sẽ xử lý version
    /// </summary>
    public class EditVersionAttribute :Attribute
    {
        /// <summary>
        /// Tên trường dữ liệu
        /// </summary>
        public string DataField { get; set; } = "edit_version";
    }
}
