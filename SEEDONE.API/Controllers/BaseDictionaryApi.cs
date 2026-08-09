using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.HPSF;
using NPOI.OpenXml4Net.OPC.Internal;
using SEEDONE.SERVICE.Contexts;
using SEEDONE.SERVICE.Constants;
using SEEDONE.SERVICE.Cruds;
using SEEDONE.SERVICE.DtoEdit;
using SEEDONE.SERVICE.Helpers;
using SEEDONE.SERVICE.Interfaces.Entities;
using SEEDONE.SERVICE.Interfaces.Service.Business;
using SEEDONE.SERVICE.Model;
using SEEDONE.SERVICE.Properties;
using SEEDONE.SERVICE.Service.Business;

namespace SEEDONE.API.Controllers
{
    /// <summary>
    /// Xửu lý các nghiệp vụ chung của đối tượng
    /// </summary>
    /// <typeparam name="TService">Service nghiệp vụ</typeparam>
    /// <typeparam name="TKey">Khóa chính của bảng</typeparam>
    /// <typeparam name="TEntity">Đối tượng nào</typeparam>
    /// <typeparam name="TEntityDtoEdit">Đối tượng edit</typeparam>
    public abstract class BaseDictionaryApi<TService, TKey, TEntity, TEntityDtoEdit> : BaseApi
        where TEntityDtoEdit : IRecordState, TEntity
        where TService : ICrudBaseService<TKey, TEntity, TEntityDtoEdit>
    {
        /// <summary>
        /// Servcie xử lý nghiệp vụ với controlelr
        /// </summary>
        protected readonly TService _service;
        protected static readonly Type EntityType = typeof(TEntity);
        protected readonly IContextService _contextService;
        protected readonly ISerializerService _serializerService;
        
        public BaseDictionaryApi(TService service, IServiceProvider serviceProvider)
        {
            _service = service;
            _contextService = serviceProvider.GetRequiredService<IContextService>();
            _serializerService = serviceProvider.GetRequiredService<ISerializerService>();
        }

        /// <summary>
        /// Lấy toàn bộ dữ liệu của bảng
        /// </summary>
        [HttpGet("fullList")]
        public async Task<IActionResult> GetFull(string? sort)
        {
            return Ok(await _service.GetListAsync(sort));
        }

        /// <summary>
        /// Lấy dữ liệu theo ID
        /// Dữu liệu này sẽ bao gồm cả Detail
        /// </summary>
        /// <param name="id">id thực thể</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEdit(TKey id)
        {
            var data = await _service.GetEditAsync(id);
            if (data == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, $"Không tìm thấy bản ghi {id}");
            }
            return Ok(data);
        }

        /// <summary>
        /// Action insert
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<IActionResult> Insert([FromBody] SaveParameter<TEntityDtoEdit, TEntity> parameter)
        {
            if (parameter != null && parameter.Model != null)
            {
                parameter.Model.state = SEEDONE.SERVICE.Constants.ModelState.Insert;
            }
            var result = await _service.SaveAsync(parameter);
            return Ok(result);
        }

        /// <summary>
        /// Action update
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpPut]
        public virtual async Task<IActionResult> Update([FromBody] SaveParameter<TEntityDtoEdit, TEntity> parameter)
        {
            if (parameter != null && parameter.Model != null)
            {
                parameter.Model.state = SEEDONE.SERVICE.Constants.ModelState.Update;
            }
            var result = await _service.SaveAsync(parameter);
            return Ok(result);
        }

        /// <summary>
        /// Xóa dữ liệu viết chung với hàm xóa nhiều luôn
        /// </summary>
        [HttpDelete]
        public virtual async Task<IActionResult> Delete([FromBody] DeleteParameter<TKey, TEntity> parameter)
        {
            var result = await _service.DeleteAsync(parameter);
            return Ok(result);
        }

        /// <summary>
        /// Lấy dữ liệu thêm mới
        /// </summary>
        [HttpGet("new")]
        public async Task<IActionResult> GetNew(string? param)
        {
            var result = await _service.GetNewAsync(param);
            return Ok(result);
        }


        /// <summary>
        /// Lấy dữ liệu bảng
        /// </summary>
        [HttpPost("dataTable")]
        public async Task<IActionResult> GetDataTable([FromBody] FilterTable param)
        {
            var result = await _service.GetDataTable(param);
            return Ok(result);
        }


        /// <summary>
        /// Lấy dữ liệu combobox
        /// </summary>
        [HttpPost("combobox")]
        public async Task<IActionResult> GetComboboxPaging([FromBody] PagingParameter param)
        {
            var result = await _service.GetComboboxPaging(param.Columns, param.Filter, param.Sort);
            return Ok(result);
        }

        /// <summary>
        /// Xuất khẩu  toàn bộ dữ liệu của bảng
        /// </summary>
        [HttpPost("Export")]
        public virtual async Task<IActionResult> Export([FromBody] FilterTable param)
        {
            var resultData = await _service.GetDataTable(param);
            var streamData = ExportLib<TEntity>.ToExcel(JsonConvert.DeserializeObject<List<TEntity>>(JsonConvert.SerializeObject(resultData.Data)), typeof(TEntity).GetProperties().Select(x => x.Name).ToList());

            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                streamData.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }
            string base64 = Convert.ToBase64String(bytes);
            //return File(streamData, "application/octet-stream", $"{typeof(TEntity).Name}.xlsx");
            return Ok(base64);
        }
    }
}
