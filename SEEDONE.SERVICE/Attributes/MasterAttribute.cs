using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEEDONE.SERVICE.Attributes
{
    /// <summary>
    /// Attribute đánh dấu tên bảng master
    /// </summary>
    public class MasterAttribute : Attribute
    {
        /// <summary>
        /// Tên bảng master
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Khởi tạo
        /// </summary>
        /// <param name="table">Tên bảng</param>
        public MasterAttribute(string name)
        {
            Name = name;
        }
    }
}
