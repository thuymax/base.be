using SEEDONE.SERVICE.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SEEDONE.SERVICE.Exceptions
{
    public class ImportException : Exception
    {
        /// <summary>
        /// Nếu có thông tin trả về thì hiển thi jcho người dùng
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// MÃ lỗi
        /// phần lớn sẽ sử dụng thông tin này để client dựa theo hiển thị message tương ứng
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// Dữu liệu trả vể đẻ client hiển thị thông báo haowcj callabck tương ứng
        /// </summary>
        public object ErrorData { get; set; }

        public string GetClientReturn()
        {
            return JsonConvert.SerializeObject(new
            {
                Error = this.ErrorCode,
                Data = this.ErrorData,
                Message = this.ErrorMessage
            });
        }
    }
}
