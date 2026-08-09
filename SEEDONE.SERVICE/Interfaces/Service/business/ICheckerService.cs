using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SEEDONE.SERVICE.DtoEdit;
using SEEDONE.SERVICE.Model;
using SEEDONE.SERVICE.Model.Business;

namespace SEEDONE.SERVICE.Interfaces.Service.Business
{
    /// <summary>
    /// Interface service Người dùng
    /// </summary>
    public interface ICheckerService : ICrudBaseService<Guid, CheckerEntity, CheckerDtoEdit>
    {
        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="model"></param>
        Task<Dictionary<string, object>> Login(LoginModel model);
        /// <summary>
        /// Đăng ký
        /// </summary>
        /// <param name="model"></param>
        Task<DAResult> Signup(SignupModel model);

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="resetPassword"></param>
        Task ResetPassword(ResetPassword resetPassword);

        /// <summary>
        /// Lấy token
        /// </summary>
        Task<Dictionary<string, object>> GetToken(Guid userId);

    }
}
