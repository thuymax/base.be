using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SEEDONE.SERVICE.Exceptions
{
    /// <summary>
    /// Xử lý ngoại lệ chung của ứng dụng
    /// </summary>
    public class CustomExceptionFilter : IActionFilter, IOrderedFilter
    {
        private readonly int BUSSINESS_ERROR_CODE = (int)HttpStatusCode.UnprocessableEntity;

        public int Order => int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is BusinessException businessException)
            {
                context.Result = new ObjectResult(businessException)
                {
                    StatusCode = BUSSINESS_ERROR_CODE,
                    Value = businessException.GetClientReturn()
                };

                context.ExceptionHandled = true;
            }
            // Bắt lỗi exception khi nhập khẩu
            else if (context.Exception is ImportException importException)
            {
                context.Result = new ObjectResult(importException)
                {
                    StatusCode = BUSSINESS_ERROR_CODE,
                    Value = importException.GetClientReturn()
                };

                context.ExceptionHandled = true;
            }
            else if (context.Exception is UnauthorizedAccessException unauthorizedAccessException)
            {
                context.Result = new ObjectResult(unauthorizedAccessException)
                {
                    StatusCode = BUSSINESS_ERROR_CODE,
                    Value = unauthorizedAccessException.Data
                };

                context.ExceptionHandled = true;
            }
            else if (context.Exception != null)
            {
                context.Result = new ObjectResult(context.Exception)
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Value = context.Exception.Message
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
