using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEEDONE.SERVICE.Constants;

namespace SEEDONE.SERVICE.Interfaces.Entities
{
    /// <summary>
    /// Xửu lý thông tin version của dữ liệu
    /// </summary>
    public interface IRecordVersion
    {
        /// <summary>
        /// version của dữ liệu
        /// Mặc định sẽ đọc thông tin trường từ ModifiedDate
        /// </summary>
        long RecordVersion { get; set; }
    }
}
