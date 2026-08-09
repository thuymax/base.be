using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SEEDONE.SERVICE.Constants;
using SEEDONE.SERVICE.Contexts;
using SEEDONE.SERVICE.Cruds;
using SEEDONE.SERVICE.DtoEdit;
using SEEDONE.SERVICE.Exceptions;
using SEEDONE.SERVICE.Interfaces.Repo.Business;
using SEEDONE.SERVICE.Interfaces.Service.Business;
using SEEDONE.SERVICE.Model;
using SEEDONE.SERVICE.Model.Business;
using SEEDONE.SERVICE.Properties;
using SEEDONE.SERVICE.Service.Business;

namespace SEEDONE.SERVICE.Service.Business
{
    public class CheckerService : CrudBaseService<ICheckerRepo, Guid, CheckerEntity, CheckerDtoEdit>, ICheckerService
    {
        ICheckerRepo _CheckerRepo;

        public CheckerService(ICheckerRepo repo, IServiceProvider serviceProvider) : base(repo, serviceProvider)
        {
            _CheckerRepo = repo;
        }

        public async Task<Dictionary<string, object>> Login(LoginModel model)
        {
            var Checker = await _CheckerRepo.Login(model);
            var context = new ContextData();
            if (Checker != null)
            {
            //    context.CheckerId = Checker.checker_id;
            //    context.CheckerName = Checker.checker_name;
            //    context.CheckerCode = Checker.checker_code;
            //    context.Email = Checker.email;
            //    context.Role = Checker.role;
            //    context.RoleDetail = Checker.role_detail;
            }

            var jwtTokenConfig =
                JsonConvert.DeserializeObject<JwtTokenConfig>(_repo.GetConfiguration()
                    .GetConnectionString("JwtTokenConfig"));

            var data = await this.GetContextReturn(context, jwtTokenConfig);
            return data;
        }

        /// <summary>
        /// Lấy thông tin trả về client
        /// </summary>
        /// <param name="Checker"></param>
        /// <param name="expiredSeconds">Thời gian hết hạn</param>
        private async Task<Dictionary<string, object>> GetContextReturn(ContextData context,
            JwtTokenConfig jwtTokenConfig)
        {
            var token = this.CreateAuthenToken(context, jwtTokenConfig);
            var result = new Dictionary<string, object>
            {
                { "Token", $"Bearer {token}" },
                { "TokenTimeout", jwtTokenConfig.ExpiredSeconds },
                //{
                //    "Context", new
                //    {
                //        CheckerId = context.CheckerId,
                //        Email = context.Email,
                //        CheckerName = context.CheckerName,
                //        CheckerCode = context.CheckerCode,
                //        TokenExpired = context.TokenExpired,
                //        Role = context.Role,
                //        RoleDetail = context.RoleDetail,
                //    }
                //}
            };
            return result;
        }

        /// <summary>
        /// Tạo token Authen
        /// </summary>
        private string CreateAuthenToken(ContextData context, JwtTokenConfig jwtTokenConfig)
        {
            var claimIdentity = new ClaimsIdentity();
            //claimIdentity.AddClaim(new Claim(TokenKeys.CheckerId, context.CheckerId.ToString()));
            //claimIdentity.AddClaim(new Claim(TokenKeys.Email, context.Email));
            //claimIdentity.AddClaim(new Claim(TokenKeys.CheckerName, context.CheckerName));
            //claimIdentity.AddClaim(new Claim(TokenKeys.CheckerCode, context.CheckerCode));
            //claimIdentity.AddClaim(new Claim(TokenKeys.Role, context.Role.ToString()));

            var expire = DateTime.Now.AddSeconds(jwtTokenConfig.ExpiredSeconds);
            //context.TokenExpired = expire;
            //claimIdentity.AddClaim(new Claim(TokenKeys.TokenExpired, context.TokenExpired.ToString()));
            var key = Encoding.ASCII.GetBytes(jwtTokenConfig.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = expire,
                Subject = claimIdentity,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken smeToken = tokenHandler.CreateToken(tokenDescriptor);
            string token = tokenHandler.WriteToken(smeToken);
            return token;
        }

        /// <summary>
        /// Reset Password
        /// </summary>
        public async Task ResetPassword(ResetPassword resetPassword)
        {
            string field = "checker_id";
            object value = resetPassword.checker_id;

            var existedChecker = await _repo.GetAsync<CheckerEntity>(field, value);
            var res = existedChecker.FirstOrDefault();

            if (res == null)
            {
                throw new ValidateException("Checker doesn't exist", "");
            }
            var verified = BCrypt.Net.BCrypt.Verify(resetPassword.password, res.password);
            if (!verified)
            {
                throw new ValidateException("Mật khẩu không chính xác, vui lòng kiểm tra lại", 0, int.Parse(ResultCode.WrongPassword));
            }
            var encodedData = BCrypt.Net.BCrypt.HashPassword(resetPassword.new_password);
            res.password = encodedData;

            await _repo.UpdateAsync<CheckerEntity>(res, "password");
        }
        
        public async Task<DAResult> Signup(SignupModel model)
        {
            var newChecker = new CheckerEntity();
            newChecker.checker_id = Guid.NewGuid();
            newChecker.email = model.email;
            newChecker.phone_number = model.phone;
            newChecker.password = BCrypt.Net.BCrypt.HashPassword(model.password);
            newChecker.checker_name = model.name.Trim();
            newChecker.checker_code = model.code.Trim();
            newChecker.role = Role.Customer;
            // Check tồn tại Email
            var existCheckerEmail = (await _repo.GetAsync<CheckerEntity>("email", newChecker.email))?.FirstOrDefault();
            if (existCheckerEmail != null)
            {
                return new DAResult(int.Parse(ResultCode.ExistEmail), Resources.msgExistEmail, "", newChecker);
            }
            // Check tồn tại Số điện thoại
            var existCheckerPhone = (await _repo.GetAsync<CheckerEntity>("phone", newChecker.phone_number))?.FirstOrDefault();
            if (existCheckerPhone != null)
            {
                return new DAResult(int.Parse(ResultCode.ExistPhone), Resources.msgExistPhone, "", newChecker);
            }
            var Checker = await _repo.InsertAsync<CheckerEntity>(newChecker);


            var context = new ContextData();
            if (Checker != null)
            {
                //context.CheckerId = Checker.checker_id;
                //context.Email = Checker.email;
                //context.CheckerCode = Checker.checker_code;
                //context.CheckerName = Checker.checker_name;
                //context.Role = Checker.role;
            }

            var jwtTokenConfig =
                JsonConvert.DeserializeObject<JwtTokenConfig>(_repo.GetConfiguration()
                    .GetConnectionString("JwtTokenConfig"));

            var data = await this.GetContextReturn(context, jwtTokenConfig);
            return new DAResult(200, Resources.signupSuccess, "", data);
        }

        public async Task<Dictionary<string, object>> GetToken(Guid CheckerId)
        {
            var Checker = await _repo.GetByIdAsync<CheckerEntity>(CheckerId);
            var context = new ContextData();
            if (Checker != null)
            {
                //context.CheckerId = Checker.checker_id;
                //context.Email = Checker.email;
                //context.CheckerCode = Checker.checker_code;
                //context.CheckerName = Checker.checker_name;
                //context.Role = Checker.role;
            }

            var jwtTokenConfig =
                JsonConvert.DeserializeObject<JwtTokenConfig>(_repo.GetConfiguration()
                    .GetConnectionString("JwtTokenConfig"));

            var data = await this.GetContextReturn(context, jwtTokenConfig);
            return data;
        }

        protected override async Task<CheckerDtoEdit> GetEditAsync(IDbConnection cnn, Guid id)
        {
            var model = await base.GetEditAsync(cnn, id);
            //if (_contextService.Get().Role != Role.Admin)
            //{
            //    model.password = null;
            //}

            return model;
        }

        protected override async Task ValidateDeleteAssync(IDbConnection cnn, DeleteParameter<Guid, CheckerEntity> parameter, CheckerEntity model)
        {
            var exist = await _repo.GetAsync<TimeSheetEntity>(nameof(TimeSheetEntity.checker_id), model.checker_id);
            if (exist?.Count > 0)
            {
                throw new BusinessException
                {
                    ErrorCode = ErorrCodes.Valdiate,
                    ErrorData = new ValidateResult
                    {
                        Type = ValidateResultType.Generation,
                        Data = new
                        {
                            Code = $"{model.checker_code} - {model.checker_name}",
                            Data = exist.Select(x => x.time_sheet_code).ToList(),
                            Type = "temesheet"
                        }
                    }
                };
            } else
            {
                var exist2 = await _repo.GetAsync<DossierEntity>(nameof(DossierEntity.checker_id), model.checker_id);
                if (exist2?.Count > 0)
                {
                    throw new BusinessException
                    {
                        ErrorCode = ErorrCodes.Valdiate,
                        ErrorData = new ValidateResult
                        {
                            Type = ValidateResultType.Generation,
                            Data = new
                            {
                                Code = $"{model.checker_code} - {model.checker_name}",
                                Data = exist2.Select(x => x.dossier_code).ToList(),
                                Type = "dossier"
                            }
                        }
                    };
                }
            }
        }
    }
}