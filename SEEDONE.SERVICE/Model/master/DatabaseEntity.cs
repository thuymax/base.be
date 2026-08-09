using SEEDONE.SERVICE.Attributes;

namespace SEEDONE.SERVICE.Model.Master
{
    /// <summary>
    /// Bảng lưu database business
    /// </summary>
    [Table("database")]
    public class DatabaseEntity
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [Key]
        public Guid database_id { get; set; }

        /// <summary>
        /// Dữ liệu này của bên nào
        /// </summary>
        [Require]
        public string database_name { get; set; }

        /// <summary>
        /// Trạng thái database
        /// </summary>
        public int? status { get; set; }

        /// <summary>
        /// Connection
        /// </summary>
        public string? connection { get; set; }

        /// <summary>
        /// Phiên bản của database
        /// </summary>
        public string? version { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime? created_date { get; set; }
    }
}

