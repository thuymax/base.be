using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEEDONE.SERVICE.Constants
{
    public enum ValidateCode
    {
        /// <summary>
        /// Bắt buộc
        /// </summary>
        Required,
        /// <summary>
        /// Lệch phiên dữ liệu
        /// </summary>
        EditVersion,
        /// <summary>
        /// Trùng mã
        /// </summary>
        Duplicate,
    }
}
