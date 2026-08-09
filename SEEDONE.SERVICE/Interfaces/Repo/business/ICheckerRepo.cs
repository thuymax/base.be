using SEEDONE.SERVICE.DtoEdit;
using SEEDONE.SERVICE.Model;
using SEEDONE.SERVICE.Model.Business;

namespace SEEDONE.SERVICE.Interfaces.Repo.Business
{
    public interface ICheckerRepo : IBaseRepo
    {
        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="model"></param>
        Task<CheckerEntity> Login(LoginModel model);
    }
}
