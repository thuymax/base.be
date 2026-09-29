using System.Text.RegularExpressions;

namespace SEEDONE.SERVICE.Helpers
{
    public static class FileHelper
    {
        private static readonly char[] InvalidChars = Path.GetInvalidFileNameChars();
        
        /// <summary>Sanitize tên file — loại bỏ ký tự nguy hiểm</summary>
        public static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return "file";
            
            var name = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            
            // Loại bỏ ký tự không hợp lệ
            name = string.Concat(name.Split(InvalidChars));
            name = Regex.Replace(name, @"\s+", "_");  // Space → underscore
            name = Regex.Replace(name, @"[^a-zA-Z0-9_\-]", ""); // Chỉ giữ alphanumeric
            
            if (string.IsNullOrWhiteSpace(name)) name = "file";
            if (name.Length > 100) name = name[..100];
            
            return name + ext;
        }
        
        /// <summary>Tạo unique file key: folder/year/month/uuid_filename</summary>
        public static string BuildFileKey(string sanitizedFileName, string? folder = null)
        {
            var now = DateTime.UtcNow;
            var uniqueId = Guid.NewGuid().ToString("N")[..8];
            var datePath = $"{now:yyyy}/{now:MM}";
            var uniqueName = $"{uniqueId}_{sanitizedFileName}";
            
            return string.IsNullOrWhiteSpace(folder)
                ? $"{datePath}/{uniqueName}"
                : $"{folder.Trim('/')}/{datePath}/{uniqueName}";
        }
        
        /// <summary>Validate extension file</summary>
        public static bool IsAllowedExtension(string fileName, string[] allowedExtensions)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return allowedExtensions.Contains(ext);
        }
        
        /// <summary>Validate file size</summary>
        public static bool IsAllowedSize(long fileSizeBytes, long maxSizeBytes)
        {
            return fileSizeBytes <= maxSizeBytes;
        }
        
        /// <summary>Detect content type từ extension</summary>
        public static string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };
        }
        
        /// <summary>Format file size for display</summary>
        public static string FormatFileSize(long bytes)
        {
            string[] sizes = ["B", "KB", "MB", "GB"];
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
