namespace SEEDONE.AUTH.Constants
{
    public static class AccountErrorMessages
    {
        public const string UserNotFound = "Tài khoản không tồn tại trong hệ thống.";
        public const string WrongPassword = "Mật khẩu không chính xác.";
        public const string MustChangePassword = "Bạn cần đổi mật khẩu trước khi tiếp tục sử dụng.";
        public const string PasswordExpired = "Mật khẩu đã hết hạn, vui lòng đổi mật khẩu mới.";
        public const string AccountLocked = "Tài khoản đã bị khóa, vui lòng liên hệ quản trị viên.";
        public const string MissingBusinessDatabase = "Tài khoản chưa được phân quyền database Business.";
        public const string RegistrationNotAllowed = "Không được phép đăng ký bằng khóa API hiện tại.";
        public const string UserAlreadyExists = "Tài khoản đã tồn tại.";
        public const string InvalidApiKey = "API key không hợp lệ.";
        public const string PasswordNotMatch = "Mật khẩu mới không được trùng mật khẩu cũ.";
        public const string TenantNotFound = "Không tìm thấy tenant với mã đã cung cấp.";
    }
}

