using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEEDONE.SERVICE.Attributes
{
    /// <summary>
    /// Thông tin tham chiếu với dữ liệu gốc
    /// </summary>
    public interface IMasterRefAttribute
    {
        /// <summary>
        /// Tên trường mapping với master key
        /// </summary>
        public string MasterKeyField { get; set; }
        /// <summary>
        /// Kiểu dl
        /// </summary>
        public Type Type { get; set; }
        /// <summary>
        /// Thứ tự xử lý
        /// </summary>
        public int Index { get; set; }
    }
}
