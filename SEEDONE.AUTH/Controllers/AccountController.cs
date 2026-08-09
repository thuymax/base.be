using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SEEDONE.AUTH.Constants;
using SEEDONE.AUTH.Models;
using SEEDONE.SERVICE.Constants;
using SEEDONE.SERVICE.Contexts;
using SEEDONE.SERVICE.DtoEdit;
using SEEDONE.SERVICE.Interfaces.Repo.Master;
using SEEDONE.SERVICE.Model.Master;

namespace SEEDONE.AUTH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IMasterBaseRepo _masterRepo;
        private readonly IConfiguration _configuration;

        public AccountController(IMasterBaseRepo masterRepo, IConfiguration configuration)
        {
            _masterRepo = masterRepo;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AccountLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var normalizedUserName = request.UserName.Trim();
            var user = (await _masterRepo.GetAsync<UserEntity>(nameof(UserEntity.user_name), normalizedUserName)).FirstOrDefault();
            if (user == null)
            {
                return BuildErrorResult(AccountErrorCodes.UserNotFound, AccountErrorMessages.UserNotFound, StatusCodes.Status401Unauthorized);
            }

            var passwordMatched = BCrypt.Net.BCrypt.Verify(request.PassWord, user.password);
            if (!passwordMatched)
            {
                return BuildErrorResult(AccountErrorCodes.WrongPassword, AccountErrorMessages.WrongPassword, StatusCodes.Status401Unauthorized);
            }

            var statusValidation = ValidateUserStatus(user);
            if (statusValidation != null)
            {
                return statusValidation;
            }

            var databaseUsers =
                await _masterRepo.GetAsync<DatabaseUserEntity>(nameof(DatabaseUserEntity.user_id), user.user_id);
            if (databaseUsers == null || databaseUsers.Count == 0)
            {
                return BuildErrorResult(AccountErrorCodes.MissingBusinessDatabase, AccountErrorMessages.MissingBusinessDatabase);
            }

            var databaseMap = await this.GetDatabaseMapAsync(databaseUsers);
            if (databaseMap.Count == 0)
            {
                return BuildErrorResult(AccountErrorCodes.MissingBusinessDatabase, AccountErrorMessages.MissingBusinessDatabase);
            }

            var databaseContexts = this.BuildDatabaseContexts(databaseUsers, databaseMap).ToList();
            if (!databaseContexts.Any())
            {
                return BuildErrorResult(AccountErrorCodes.MissingBusinessDatabase, AccountErrorMessages.MissingBusinessDatabase);
            }

            var tenants =
                await _masterRepo.GetAsync<TenantEntity>(nameof(TenantEntity.tenant_id), databaseUsers.First().tenant_id);
            var firstDatabaseContext = databaseContexts.First();
       

            var context = this.BuildContext(user, firstDatabaseContext, tenants.First());
            var jwtTokenConfig = this.GetJwtConfig();
            var token = this.CreateAuthToken(context, jwtTokenConfig);

            var response = new AccountLoginResponse
            {
                Token = $"Bearer {token}",
                TokenTimeout = jwtTokenConfig.ExpiredSeconds,
                Context = context,
                Databases = databaseContexts
            };

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AccountRegisterRequest request, [FromHeader(Name = "x-api-key")] string? apiKey)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (!IsValidRegistrationKey(apiKey))
            {
                return BuildErrorResult(AccountErrorCodes.InvalidApiKey, AccountErrorMessages.InvalidApiKey, StatusCodes.Status401Unauthorized);
            }

            var normalizedUserName = request.UserName.Trim();
            var existed = (await _masterRepo.GetAsync<UserEntity>(nameof(UserEntity.user_name), normalizedUserName)).FirstOrDefault();
            if (existed != null)
            {
                return Conflict(new { code = AccountErrorCodes.UserAlreadyExists, message = AccountErrorMessages.UserAlreadyExists });
            }

            var normalizedTenantCode = request.TenantCode.Trim();
            var tenant = (await _masterRepo.GetAsync<TenantEntity>(nameof(TenantEntity.tenant_code), normalizedTenantCode)).FirstOrDefault();
            if (tenant == null)
            {
                return BuildErrorResult(AccountErrorCodes.TenantNotFound, AccountErrorMessages.TenantNotFound, StatusCodes.Status404NotFound);
            }

            var existingDatabaseUser = (await _masterRepo.GetAsync<DatabaseUserEntity>(nameof(DatabaseUserEntity.tenant_id), tenant.tenant_id)).FirstOrDefault();
            if (existingDatabaseUser == null)
            {
                return BuildErrorResult(AccountErrorCodes.MissingBusinessDatabase, AccountErrorMessages.MissingBusinessDatabase);
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.PassWord);
            var newUser = new UserEntity
            {
                user_id = Guid.NewGuid(),
                user_name = normalizedUserName,
                password = hashedPassword,
                full_name = request.FullName?.Trim(),
                email = request.Email,
                phone_number = request.PhoneNumber,
                status = 0,
                created_date = DateTime.UtcNow,
                created_by = request.UserName
            };

            var inserted = await _masterRepo.InsertAsync<UserEntity>(newUser) ?? newUser;

            var newDatabaseUser = new DatabaseUserEntity
            {
                database_user_id = Guid.NewGuid(),
                user_id = inserted.user_id,
                database_id = existingDatabaseUser.database_id,
                tenant_id = tenant.tenant_id,
                created_date = DateTime.UtcNow
            };

            await _masterRepo.InsertAsync<DatabaseUserEntity>(newDatabaseUser);

            return StatusCode(StatusCodes.Status201Created, new
            {
                inserted.user_id,
                inserted.user_name,
                inserted.full_name,
                inserted.email,
                inserted.phone_number,
                tenant_id = tenant.tenant_id,
                tenant_code = tenant.tenant_code
            });
        }

        [AllowAnonymous]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var normalizedUserName = request.UserName.Trim();
            var user = (await _masterRepo.GetAsync<UserEntity>(nameof(UserEntity.user_name), normalizedUserName)).FirstOrDefault();
            if (user == null)
            {
                return BuildErrorResult(AccountErrorCodes.UserNotFound, AccountErrorMessages.UserNotFound, StatusCodes.Status404NotFound);
            }

            var verified = BCrypt.Net.BCrypt.Verify(request.OldPassword, user.password);
            if (!verified)
            {
                return BuildErrorResult(AccountErrorCodes.WrongPassword, AccountErrorMessages.WrongPassword, StatusCodes.Status401Unauthorized);
            }

            if (BCrypt.Net.BCrypt.Verify(request.NewPassword, user.password))
            {
                return BuildErrorResult(AccountErrorCodes.PasswordNotMatch, AccountErrorMessages.PasswordNotMatch);
            }

            user.password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.status = 0;
            user.modified_date = DateTime.UtcNow;
            user.modified_by = user.user_name;
            await _masterRepo.UpdateAsync<UserEntity>(user, "password,status,modified_date,modified_by");

            return Ok(new { message = "Đổi mật khẩu thành công" });
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, [FromHeader(Name = "x-api-key")] string? apiKey)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (!IsValidRegistrationKey(apiKey))
            {
                return BuildErrorResult(AccountErrorCodes.InvalidApiKey, AccountErrorMessages.InvalidApiKey, StatusCodes.Status401Unauthorized);
            }

            var normalizedUserName = request.UserName.Trim();
            var user = (await _masterRepo.GetAsync<UserEntity>(nameof(UserEntity.user_name), normalizedUserName)).FirstOrDefault();
            if (user == null)
            {
                return BuildErrorResult(AccountErrorCodes.UserNotFound, AccountErrorMessages.UserNotFound, StatusCodes.Status404NotFound);
            }

            user.password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.status = 0;
            user.modified_date = DateTime.UtcNow;
            user.modified_by = user.user_name;
            await _masterRepo.UpdateAsync<UserEntity>(user, "password,status,modified_date,modified_by");

            return Ok(new { message = "Đặt lại mật khẩu thành công" });
        }

        private ContextData BuildContext(UserEntity user, LoginDatabaseContext defaultDatabase, TenantEntity? tenant)
        {
            return new ContextData
            {
                Email = user.email,
                UserId = user.user_id,
                UserName = user.user_name,
                FullName = user.full_name,
                PhoneNumber = user.phone_number,
                Status = user.status,
                TenantId = defaultDatabase.TenantId,
                TenantCode = tenant?.tenant_code,
                DatabaseId = defaultDatabase.DatabaseId,
                DatabaseName = defaultDatabase.DatabaseName
            };
        }

        private JwtTokenConfig GetJwtConfig()
        {
            var rawConfig = _configuration.GetConnectionString("JwtTokenConfig");
            if (string.IsNullOrWhiteSpace(rawConfig))
            {
                throw new InvalidOperationException("Thiếu cấu hình JwtTokenConfig trong appsettings.");
            }

            var config = JsonConvert.DeserializeObject<JwtTokenConfig>(rawConfig);
            if (config == null)
            {
                throw new InvalidOperationException("JwtTokenConfig không hợp lệ.");
            }

            return config;
        }

        private string CreateAuthToken(ContextData context, JwtTokenConfig jwtTokenConfig)
        {
            var claims = new List<Claim>();
            
            if (!string.IsNullOrWhiteSpace(context.Email))
            {
                claims.Add(new Claim(TokenKeys.Email, context.Email));
            }
            if (context.UserId.HasValue)
            {
                claims.Add(new Claim(TokenKeys.UserId, context.UserId.Value.ToString()));
            }
            if (!string.IsNullOrWhiteSpace(context.UserName))
            {
                claims.Add(new Claim(TokenKeys.UserName, context.UserName));
            }
            if (!string.IsNullOrWhiteSpace(context.FullName))
            {
                claims.Add(new Claim(TokenKeys.FullName, context.FullName));
            }
            if (!string.IsNullOrWhiteSpace(context.PhoneNumber))
            {
                claims.Add(new Claim(TokenKeys.PhoneNumber, context.PhoneNumber));
            }
            if (context.Status.HasValue)
            {
                claims.Add(new Claim(TokenKeys.Status, context.Status.Value.ToString()));
            }
            if (context.TenantId.HasValue)
            {
                claims.Add(new Claim(TokenKeys.TenantId, context.TenantId.Value.ToString()));
            }
            if (!string.IsNullOrWhiteSpace(context.TenantCode))
            {
                claims.Add(new Claim(TokenKeys.TenantCode, context.TenantCode));
            }
            if (context.DatabaseId.HasValue)
            {
                claims.Add(new Claim(TokenKeys.DatabaseId, context.DatabaseId.Value.ToString()));
            }
            if (!string.IsNullOrWhiteSpace(context.DatabaseName))
            {
                claims.Add(new Claim(TokenKeys.DatabaseName, context.DatabaseName));
            }

            var expire = DateTime.UtcNow.AddSeconds(jwtTokenConfig.ExpiredSeconds);
            var key = Encoding.ASCII.GetBytes(jwtTokenConfig.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expire,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<Dictionary<Guid, DatabaseEntity>> GetDatabaseMapAsync(IEnumerable<DatabaseUserEntity> databaseUsers)
        {
            var tasks = databaseUsers.Select(async du =>
            {
                var db = await _masterRepo.GetByIdAsync<DatabaseEntity>(du.database_id);
                return db;
            });

            var databases = await Task.WhenAll(tasks);
            return databases
                .Where(db => db != null)
                .GroupBy(db => db!.database_id)
                .ToDictionary(group => group.Key, group => group.First()!);
        }

        private IEnumerable<LoginDatabaseContext> BuildDatabaseContexts(IEnumerable<DatabaseUserEntity> databaseUsers,
            IReadOnlyDictionary<Guid, DatabaseEntity> databaseMap)
        {
            var index = 0;
            foreach (var databaseUser in databaseUsers)
            {
                if (!databaseMap.TryGetValue(databaseUser.database_id, out var database))
                {
                    continue;
                }

                yield return new LoginDatabaseContext
                {
                    DatabaseUserId = databaseUser.database_user_id,
                    DatabaseId = database.database_id,
                    TenantId = databaseUser.tenant_id,
                    DatabaseName = database.database_name,
                    Connection = database.connection,
                    Version = database.version,
                    IsDefault = index++ == 0
                };
            }
        }

        private IActionResult? ValidateUserStatus(UserEntity user)
        {
            return user.status switch
            {
                1 => BuildErrorResult(AccountErrorCodes.MustChangePassword, AccountErrorMessages.MustChangePassword, StatusCodes.Status403Forbidden),
                2 => BuildErrorResult(AccountErrorCodes.PasswordExpired, AccountErrorMessages.PasswordExpired, StatusCodes.Status403Forbidden),
                3 => BuildErrorResult(AccountErrorCodes.AccountLocked, AccountErrorMessages.AccountLocked, StatusCodes.Status423Locked),
                _ => null
            };
        }

        private IActionResult BuildErrorResult(string code, string message, int statusCode = StatusCodes.Status400BadRequest)
        {
            return StatusCode(statusCode, new { code, message });
        }

        private bool IsValidRegistrationKey(string? providedKey)
        {
            var configuredKey = _configuration.GetValue<string>("AuthOptions:RegistrationApiKey");
            return !string.IsNullOrWhiteSpace(providedKey)
                   && !string.IsNullOrWhiteSpace(configuredKey)
                   && providedKey!.Equals(configuredKey, StringComparison.Ordinal);
        }
    }
}

