using SEEDONE.SERVICE.Attributes;

namespace SEEDONE.SERVICE.Model.Master
{
    /// <summary>
    /// Bảng lưu vai trò của từng user và từng công ty
    /// </summary>
    [Table("user_role")]
    public class UserRoleEntity
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        public int user_role_id { get; set; }

        /// <summary>
        /// Khóa bảng tenant
        /// </summary>
        [Require]
        public Guid tenant_id { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        [Require]
        public Guid user_id { get; set; }

        /// <summary>
        /// ID vai trò
        /// </summary>
        [Require]
        public Guid role_id { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime? created_date { get; set; }
    }
}

