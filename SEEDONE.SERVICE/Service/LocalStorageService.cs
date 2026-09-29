using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SEEDONE.SERVICE.Config;
using SEEDONE.SERVICE.Helpers;
using SEEDONE.SERVICE.Interfaces.Service;

namespace SEEDONE.SERVICE.Service
{
    public class LocalStorageService : IStorageService
    {
        private readonly StorageOptions _options;
        private readonly ILogger<LocalStorageService> _logger;

        public LocalStorageService(IOptions<StorageOptions> options, ILogger<LocalStorageService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task<StorageResult> UploadAsync(Stream stream, string fileName, string? folder = null)
        {
            try
            {
                var safeFileName = FileHelper.SanitizeFileName(fileName);
                var fileKey = FileHelper.BuildFileKey(safeFileName, folder);
                var fullPath = Path.Combine(_options.LocalBasePath, fileKey.Replace('/', Path.DirectorySeparatorChar));

                // Tạo thư mục nếu chưa tồn tại
                var dir = Path.GetDirectoryName(fullPath)!;
                Directory.CreateDirectory(dir);

                // Ghi file
                await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                stream.Position = 0;
                await stream.CopyToAsync(fileStream);

                var publicUrl = $"{_options.LocalBaseUrl.TrimEnd('/')}/{fileKey}";

                return new StorageResult
                {
                    Success = true,
                    FileKey = fileKey,
                    PublicUrl = publicUrl,
                    FileName = safeFileName,
                    FileSizeBytes = stream.Length,
                    ContentType = FileHelper.GetContentType(fileName)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi upload file local: {FileName}", fileName);
                return StorageResult.Fail(ex.Message);
            }
        }

        public async Task<bool> DeleteAsync(string fileKey)
        {
            try
            {
                var fullPath = Path.Combine(_options.LocalBasePath, fileKey.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa file local: {FileKey}", fileKey);
                return false;
            }
        }

        public string GetPublicUrl(string fileKey)
        {
            return $"{_options.LocalBaseUrl.TrimEnd('/')}/{fileKey}";
        }

        public async Task<Stream?> DownloadAsync(string fileKey)
        {
            var fullPath = Path.Combine(_options.LocalBasePath, fileKey.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(fullPath)) return null;
            return await Task.FromResult<Stream>(new FileStream(fullPath, FileMode.Open, FileAccess.Read));
        }
    }
}
