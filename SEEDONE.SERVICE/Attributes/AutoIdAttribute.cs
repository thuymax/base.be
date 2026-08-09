using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEEDONE.SERVICE.Constants;

namespace SEEDONE.SERVICE.Attributes
{
    /// <summary>
    /// Attribute dánh dấu thuộc tính là mã của đối tượng
    /// xử lý lấy mới giá trị theo autoid khi thêm mới/nhân bản
    /// </summary>
    public class AutoIdAttribute : Attribute
    {
        public AutoID CategoryID { get; set; }

        /// <summary>
        /// nếu mà người dugnf nhập chỉ có prefix mà k cosvalue (ví dụ SBB) thì dự vào biến này để sinh mã tự tăng và độ dài value là theo cấu hình truyền vào
        /// muốn 5 chữu số thì lần sau sẽ là SBB00001
        /// muốn 2 chữu số thì lần sau là SBB01
        /// mặc định lengthvalue = 0
        /// </summary>
        public int LengthValue { get; set; } = 0;

        public AutoIdAttribute(AutoID categoryID, int lengthValue = 0)
        {
            CategoryID = categoryID;
            LengthValue = lengthValue;
        }
    }
}
