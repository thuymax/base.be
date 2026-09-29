using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SEEDONE.SERVICE.Config;
using SEEDONE.SERVICE.Helpers;
using SEEDONE.SERVICE.Interfaces.Service;

namespace SEEDONE.SERVICE.Service
{
    public class S3StorageService : IStorageService
    {
        private readonly StorageOptions _options;
        private readonly ILogger<S3StorageService> _logger;
        private readonly IAmazonS3 _s3Client;

        public S3StorageService(IOptions<StorageOptions> options, ILogger<S3StorageService> logger)
        {
            _options = options.Value;
            _logger = logger;
            
            var credentials = new BasicAWSCredentials(_options.S3AccessKey, _options.S3SecretKey);
            var region = RegionEndpoint.GetBySystemName(_options.S3Region);
            _s3Client = new AmazonS3Client(credentials, region);
        }

        public async Task<StorageResult> UploadAsync(Stream stream, string fileName, string? folder = null)
        {
            try
            {
                var safeFileName = FileHelper.SanitizeFileName(fileName);
                var fileKey = FileHelper.BuildFileKey(safeFileName, folder);
                var contentType = FileHelper.GetContentType(fileName);

                stream.Position = 0;
                var request = new PutObjectRequest
                {
                    BucketName = _options.S3BucketName,
                    Key = fileKey,
                    InputStream = stream,
                    ContentType = contentType,
                    // Dùng presigned URL nếu cần private access
                    // Mặc định private — truy cập qua presigned URL hoặc CloudFront
                };

                await _s3Client.PutObjectAsync(request);

                var publicUrl = string.IsNullOrWhiteSpace(_options.S3BaseUrl)
                    ? $"https://{_options.S3BucketName}.s3.{_options.S3Region}.amazonaws.com/{fileKey}"
                    : $"{_options.S3BaseUrl.TrimEnd('/')}/{fileKey}";

                return new StorageResult
                {
                    Success = true,
                    FileKey = fileKey,
                    PublicUrl = publicUrl,
                    FileName = safeFileName,
                    FileSizeBytes = stream.Length,
                    ContentType = contentType
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi upload file lên S3: {FileName}", fileName);
                return StorageResult.Fail(ex.Message);
            }
        }

        public async Task<bool> DeleteAsync(string fileKey)
        {
            try
            {
                var request = new DeleteObjectRequest
                {
                    BucketName = _options.S3BucketName,
                    Key = fileKey
                };
                await _s3Client.DeleteObjectAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa file S3: {FileKey}", fileKey);
                return false;
            }
        }

        public string GetPublicUrl(string fileKey)
        {
            if (!string.IsNullOrWhiteSpace(_options.S3BaseUrl))
                return $"{_options.S3BaseUrl.TrimEnd('/')}/{fileKey}";
            return $"https://{_options.S3BucketName}.s3.{_options.S3Region}.amazonaws.com/{fileKey}";
        }

        /// <summary>
        /// Tạo presigned URL — link tạm thời có thời hạn (dùng cho private files)
        /// </summary>
        public string GetPresignedUrl(string fileKey, int expiryMinutes = 60)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _options.S3BucketName,
                Key = fileKey,
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes)
            };
            return _s3Client.GetPreSignedURL(request);
        }

        public async Task<Stream?> DownloadAsync(string fileKey)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = _options.S3BucketName,
                    Key = fileKey
                };
                var response = await _s3Client.GetObjectAsync(request);
                return response.ResponseStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi download file S3: {FileKey}", fileKey);
                return null;
            }
        }
    }
}
