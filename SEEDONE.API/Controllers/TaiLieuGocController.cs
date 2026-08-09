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
    public class TaiLieuGocController : BaseDictionaryApi<ITaiLieuGocService, Guid, TaiLieuGocEntity, TaiLieuGocDtoEdit>
    {
        public TaiLieuGocController(ITaiLieuGocService bookmarkTypeService, IServiceProvider serviceProvider) : base(bookmarkTypeService, serviceProvider)
        {
        }
        [HttpPost("Export")]
        public override async Task<IActionResult> Export([FromBody] FilterTable param)
        {
            var resultData = await _service.GetDataTable(param);
            MemoryStream streamData = ExportLib<ExportTaiLieuGoc>.ToTemplateExcel(JsonConvert.DeserializeObject<List<ExportTaiLieuGoc>>(JsonConvert.SerializeObject(resultData.Data)), typeof(ExportTaiLieuGoc).GetProperties().Select(x => x.Name).ToList(), "/ExcelTemplate/Template_TaiLieuGoc.xlsx");

            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                streamData.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }
            string base64 = Convert.ToBase64String(bytes);
            return Ok(base64);
        }
    }
}
