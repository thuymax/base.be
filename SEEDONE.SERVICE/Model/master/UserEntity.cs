using SEEDONE.SERVICE.Attributes;
using SEEDONE.SERVICE.Interfaces.Entities;

namespace SEEDONE.SERVICE.Model.Master
{
    /// <summary>
    /// Bảng user
    /// </summary>
    [Table("user")]
    public class UserEntity : IRecordCreate, IRecordModify
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        public Guid user_id { get; set; }

        /// <summary>
        /// UserName
        /// </summary>
        [Require]
        public string user_name { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        [Require]
        public string password { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string? email { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        public string? phone_number { get; set; }

        /// <summary>
        /// Họ và tên
        /// </summary>
        public string? full_name { get; set; }

        /// <summary>
        /// Trạng thái của user: 0 đã kích hoạt, đang hoạt động, 1 cần đổi lại mật khẩu, 2 mật khẩu hết hạn cần đổi lại, 3 tài khoản bị khóa
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime? created_date { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public string? created_by { get; set; }

        /// <summary>
        /// Ngày sửa
        /// </summary>
        public DateTime? modified_date { get; set; }

        /// <summary>
        /// Người sửa
        /// </summary>
        public string? modified_by { get; set; }
    }
}

