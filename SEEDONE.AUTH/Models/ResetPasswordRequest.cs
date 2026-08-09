using System.ComponentModel.DataAnnotations;

namespace SEEDONE.AUTH.Models
{
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "userName không được để trống")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "newPassword không được để trống")]
        public string NewPassword { get; set; } = string.Empty;
    }
}

