using SEEDONE.SERVICE.Attributes;

namespace SEEDONE.SERVICE.Model.Master
{
    /// <summary>
    /// Bảng lưu thông tin của đơn vị (công ty)
    /// </summary>
    [Table("tenant")]
    public class TenantEntity
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        public Guid tenant_id { get; set; }

        /// <summary>
        /// Mã công ty
        /// </summary>
        [Require]
        public string tenant_code { get; set; }

        /// <summary>
        /// Tên công ty
        /// </summary>
        public string? tenant_name { get; set; }

        /// <summary>
        /// Loại công ty
        /// </summary>
        public int? tenant_type { get; set; }

        /// <summary>
        /// Mã số thuế
        /// </summary>
        public string? tax_code { get; set; }

        /// <summary>
        /// Địa chỉ công ty
        /// </summary>
        public string? address { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime? created_date { get; set; }
    }
}

