using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SEEDONE.SERVICE.Contexts;
using SEEDONE.SERVICE.DtoEdit;
using SEEDONE.SERVICE.DtoEdit.ExportEntity;
using SEEDONE.SERVICE.Exceptions;
using SEEDONE.SERVICE.Helpers;
using SEEDONE.SERVICE.Interfaces.Repo.Business;
using SEEDONE.SERVICE.Interfaces.Service.Business;
using SEEDONE.SERVICE.Model;
using SEEDONE.SERVICE.Model.Business;
using SEEDONE.SERVICE.Properties;

namespace SEEDONE.API.Controllers
{
    public class CustomerController : BaseDictionaryApi<ICustomerService, Guid, CustomerEntity, CustomerDtoEdit>
    {
        public CustomerController(ICustomerService bookmarkTypeService, IServiceProvider serviceProvider) : base(bookmarkTypeService, serviceProvider)
        {
        }
        [HttpPost("Export")]
        public override async Task<IActionResult> Export([FromBody] FilterTable param)
        {
            var resultData = await _service.GetDataTable(param);
            MemoryStream streamData = ExportLib<ExportCustomer>.ToTemplateExcel(JsonConvert.DeserializeObject<List<ExportCustomer>>(JsonConvert.SerializeObject(resultData.Data)), typeof(ExportCustomer).GetProperties().Select(x => x.Name).ToList(), "/ExcelTemplate/Template_Customer.xlsx");

            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                streamData.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }
            string base64 = Convert.ToBase64String(bytes);
            return Ok(base64);
        }

        /// <summary>
        /// API test để lấy ContextData từ token
        /// </summary>
        [HttpGet("test-context")]
        public IActionResult GetContext()
        {
            var context = _contextService.Get();
            if (context == null)
            {
                return Unauthorized(new { message = "Không tìm thấy thông tin context. Vui lòng đăng nhập lại." });
            }

            return Ok(new
            {
                message = "Lấy ContextData thành công",
                context = new
                {
                    Email = context.Email,
                    UserId = context.UserId,
                    UserName = context.UserName,
                    FullName = context.FullName,
                    PhoneNumber = context.PhoneNumber,
                    Status = context.Status,
                    TenantId = context.TenantId,
                    TenantCode = context.TenantCode,
                    DatabaseId = context.DatabaseId,
                    DatabaseName = context.DatabaseName
                }
            });
        }
    }
}
