using SEEDONE.SERVICE.Attributes;
using SEEDONE.SERVICE.Interfaces.Entities;

namespace SEEDONE.SERVICE.Model.Master
{
    /// <summary>
    /// Bảng lưu vai trò của từng đơn vị
    /// </summary>
    [Table("role")]
    public class RoleEntity : IRecordCreate
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        public Guid role_id { get; set; }

        /// <summary>
        /// Khóa bảng tenant
        /// </summary>
        [Require]
        public Guid tenant_id { get; set; }

        /// <summary>
        /// Mã vai trò
        /// </summary>
        [Require]
        public string role_code { get; set; }

        /// <summary>
        /// Tên vai trò
        /// </summary>
        public string? role_name { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime? created_date { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public string? created_by { get; set; }
    }
}

