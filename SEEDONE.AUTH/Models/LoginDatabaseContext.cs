using System;
using System.Text.Json.Serialization;

namespace SEEDONE.AUTH.Models
{
    public class LoginDatabaseContext
    {
        public Guid DatabaseUserId { get; set; }
        public Guid DatabaseId { get; set; }
        public Guid TenantId { get; set; }
        public string DatabaseName { get; set; } = string.Empty;

        [JsonIgnore]
        public string? Connection { get; set; }
        public string? Version { get; set; }
        public bool IsDefault { get; set; }
    }
}

