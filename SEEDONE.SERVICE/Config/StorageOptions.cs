namespace SEEDONE.SERVICE.Config
{
    public class StorageOptions
    {
        public const string SectionName = "Storage";
        
        /// <summary>Nhà cung cấp: "local" hoặc "s3"</summary>
        public string Provider { get; set; } = "local";
        
        // Local storage
        public string LocalBasePath { get; set; } = "/app/uploads";
        public string LocalBaseUrl { get; set; } = "";
        
        // AWS S3
        public string S3BucketName { get; set; } = "";
        public string S3Region { get; set; } = "ap-southeast-1";
        public string S3AccessKey { get; set; } = "";
        public string S3SecretKey { get; set; } = "";
        public string S3BaseUrl { get; set; } = "";
        
        // Limits
        public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024; // 10MB default
        public string[] AllowedExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx"];
    }
}
