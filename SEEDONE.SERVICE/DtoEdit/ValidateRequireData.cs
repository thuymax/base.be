using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEEDONE.SERVICE.Constants;

namespace SEEDONE.SERVICE.DtoEdit
{
    /// <summary>
    /// THông tin lỗi nhập thiếu dl
    /// </summary>
    public class ValidateRequireData
    {
        /// <summary>
        /// Vị trí bản ghi
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Ds các trg nhập thiếu
        /// </summary>
        public List<string> Fields { get; set; }
    }
}
