namespace SEEDONE.SERVICE.Interfaces.Service
{
    public class StorageResult
    {
        public bool Success { get; set; }
        /// <summary>Key/path duy nhất trong storage system</summary>
        public string FileKey { get; set; } = "";
        /// <summary>URL công khai để truy cập file</summary>
        public string PublicUrl { get; set; } = "";
        public string FileName { get; set; } = "";
        public long FileSizeBytes { get; set; }
        public string ContentType { get; set; } = "";
        public string? ErrorMessage { get; set; }
        
        public static StorageResult Fail(string error) => new() { Success = false, ErrorMessage = error };
    }

    public interface IStorageService
    {
        /// <summary>Upload file. Trả về StorageResult chứa URL.</summary>
        Task<StorageResult> UploadAsync(Stream stream, string fileName, string? folder = null);
        
        /// <summary>Xóa file theo key.</summary>
        Task<bool> DeleteAsync(string fileKey);
        
        /// <summary>Lấy public URL từ file key.</summary>
        string GetPublicUrl(string fileKey);
        
        /// <summary>Download file về stream.</summary>
        Task<Stream?> DownloadAsync(string fileKey);
    }
}
