using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEEDONE.SERVICE.Interfaces.Entities
{
    public interface IImportModel
    {
        /// <summary>
        /// Danh sách lỗi của bản ghi
        /// </summary>
        List<ErrorDetail> ErrorDetails { get; set; }
        /// <summary>
        /// Trang thái validate
        /// </summary>
        int ImportValidateStatus { get; set; }
        /// <summary>
        /// Dòng map với entity
        /// </summary>
        int? RowImportIndex { get; set; }
        /// <summary>
        /// có update dư thừa hay không
        /// </summary>
        bool IsUpdateRedudant { get; set; }
        /// <summary>
        /// Dữ liệu cũ của bản ghi
        /// </summary>
        IDictionary<string, object> OldData { get; set; }
        /// <summary>
        /// Dùng trong trường hợp nhập khẩu detail mẫu ngang
        /// </summary>
        int? HorizontalDetailId { get; set; }
    }
    public class ErrorDetail
    {
        /// <summary>
        /// Thứ tự cột
        /// </summary>
        public int ColumnIndex { get; set; }
        /// <summary>
        /// tên cột
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// loại lỗi
        /// </summary>
        public int ErrorType { get; set; }
        /// <summary>
        /// Chuỗi thông báo lỗi
        /// </summary>
        public string MessageError { get; set; }
        /// <summary>
        /// Thứ tự dòng lỗi
        /// </summary>
        public int RowIndex { get; set; }
        /// <summary>
        /// Thứ tự Sheet trên file excel
        /// </summary>
        public int SheetIndex { get; set; }
    }
}
