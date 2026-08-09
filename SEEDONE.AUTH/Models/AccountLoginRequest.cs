using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SEEDONE.AUTH.Models
{
    public class AccountLoginRequest
    {
        [Required(ErrorMessage = "userName không được để trống")]
        [JsonPropertyName("userName")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "passWord không được để trống")]
        [JsonPropertyName("passWord")]
        public string PassWord { get; set; } = string.Empty;
    }
}

