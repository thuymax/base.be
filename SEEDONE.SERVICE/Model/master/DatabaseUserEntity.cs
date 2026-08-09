using SEEDONE.SERVICE.Attributes;

namespace SEEDONE.SERVICE.Model.Master
{
    /// <summary>
    /// Bảng lưu thông tin kết nối giữa user và database
    /// </summary>
    [Table("database_user")]
    public class DatabaseUserEntity
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        public Guid database_user_id { get; set; }

        /// <summary>
        /// Khóa bảng user
        /// </summary>
        [Require]
        public Guid user_id { get; set; }

        /// <summary>
        /// Khóa bảng database
        /// </summary>
        [Require]
        public Guid database_id { get; set; }

        /// <summary>
        /// Khóa bảng tenant
        /// </summary>
        [Require]
        public Guid tenant_id { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime? created_date { get; set; }
    }
}

