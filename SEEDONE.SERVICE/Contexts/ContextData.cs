using System.Text.Json.Serialization;

namespace SEEDONE.SERVICE.Contexts
{
    public class ContextData
    {
        /// <summary>
        /// Email người dùng
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Thông tin người dùng master
        /// </summary>
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public int? Status { get; set; }

        /// <summary>
        /// Thông tin database business mà user được gán
        /// </summary>
        public Guid? TenantId { get; set; }
        public string? TenantCode { get; set; }
        public Guid? DatabaseId { get; set; }
        public string? DatabaseName { get; set; }
    }
}
