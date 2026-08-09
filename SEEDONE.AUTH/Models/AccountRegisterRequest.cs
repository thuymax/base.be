using System.ComponentModel.DataAnnotations;

namespace SEEDONE.AUTH.Models
{
    public class AccountRegisterRequest
    {
        [Required(ErrorMessage = "userName không được để trống")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "passWord không được để trống")]
        public string PassWord { get; set; } = string.Empty;

        [Required(ErrorMessage = "fullName không được để trống")]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "email không hợp lệ")]
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "tenantCode không được để trống")]
        public string TenantCode { get; set; } = string.Empty;
    }
}

