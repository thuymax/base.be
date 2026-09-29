using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEEDONE.SERVICE.Config;
using SEEDONE.SERVICE.Helpers;
using SEEDONE.SERVICE.Interfaces.Service;
using SEEDONE.SERVICE.Model;
using Microsoft.Extensions.Options;

namespace SEEDONE.API.Controllers
{
    /// <summary>
    /// Upload và quản lý file (ảnh, tài liệu)
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly StorageOptions _storageOptions;

        public StorageController(IStorageService storageService, IOptions<StorageOptions> options)
        {
            _storageService = storageService;
            _storageOptions = options.Value;
        }

        /// <summary>Upload 1 file</summary>
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string? folder = null)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new DAResult(400, "File không hợp lệ", "File is null or empty", null));

            // Validate extension
            if (!FileHelper.IsAllowedExtension(file.FileName, _storageOptions.AllowedExtensions))
                return BadRequest(new DAResult(400,
                    $"Định dạng file không được hỗ trợ. Hỗ trợ: {string.Join(", ", _storageOptions.AllowedExtensions)}",
                    "File extension not allowed", null));

            // Validate size
            if (!FileHelper.IsAllowedSize(file.Length, _storageOptions.MaxFileSizeBytes))
                return BadRequest(new DAResult(400,
                    $"File vượt quá kích thước cho phép ({FileHelper.FormatFileSize(_storageOptions.MaxFileSizeBytes)})",
                    "File size exceeded", null));

            await using var stream = file.OpenReadStream();
            var result = await _storageService.UploadAsync(stream, file.FileName, folder);

            if (!result.Success)
                return StatusCode(500, new DAResult(500, "Upload thất bại", result.ErrorMessage, null));

            return Ok(new DAResult(200, "Upload thành công", null, result));
        }

        /// <summary>Upload nhiều file cùng lúc</summary>
        [HttpPost("upload-multiple")]
        public async Task<IActionResult> UploadMultiple(List<IFormFile> files, [FromQuery] string? folder = null)
        {
            if (files == null || files.Count == 0)
                return BadRequest(new DAResult(400, "Không có file nào được chọn", null, null));

            var results = new List<object>();
            foreach (var file in files)
            {
                if (file.Length == 0) continue;
                if (!FileHelper.IsAllowedExtension(file.FileName, _storageOptions.AllowedExtensions))
                {
                    results.Add(new { file.FileName, Success = false, Error = "Extension không được hỗ trợ" });
                    continue;
                }
                await using var stream = file.OpenReadStream();
                var result = await _storageService.UploadAsync(stream, file.FileName, folder);
                results.Add(new { file.FileName, result.Success, result.PublicUrl, result.FileKey, result.ErrorMessage });
            }

            return Ok(new DAResult(200, "Xử lý upload hoàn tất", null, results));
        }

        /// <summary>Xóa file theo key</summary>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] string fileKey)
        {
            if (string.IsNullOrWhiteSpace(fileKey))
                return BadRequest(new DAResult(400, "FileKey không hợp lệ", null, null));

            var success = await _storageService.DeleteAsync(fileKey);
            if (!success)
                return NotFound(new DAResult(404, "Không tìm thấy file", null, null));

            return Ok(new DAResult(200, "Xóa file thành công", null, null));
        }
    }
}
