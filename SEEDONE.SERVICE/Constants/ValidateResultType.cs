using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEEDONE.SERVICE.Constants
{
    /// <summary>
    /// Các loại kết quả valdiate trả về lỗi
    /// </summary>
    public enum ValidateResultType
    {
        /// <summary>
        /// Lỗi
        /// Hiển thị thông báo dừng lại
        /// </summary>
        Error = 0,
        /// <summary>
        /// Cảnh báo ấn OK
        /// </summary>
        Warning = 1,
        /// <summary>
        /// Hiển thị confirm yes/no
        /// </summary>
        Question = 2,
        /// <summary>
        /// Lỗi phát sinh dữ liệu
        /// </summary>
        Generation = 3,
    }
}
