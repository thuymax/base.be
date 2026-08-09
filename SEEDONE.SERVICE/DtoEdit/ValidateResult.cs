using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEEDONE.SERVICE.Constants;

namespace SEEDONE.SERVICE.DtoEdit
{
    public class ValidateResult
    {
        /// <summary>
        /// MÃ nghiệp vụ
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Loại gì
        /// </summary>
        public ValidateResultType Type { get; set; }
        /// <summary>
        /// Nội dung trả về client
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Dữ liệu trả về kèm theo
        /// </summary>
        public object Data { get; set; }
    }
}
