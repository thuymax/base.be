using System;
using System.Collections.Generic;
using SEEDONE.SERVICE.Contexts;

namespace SEEDONE.AUTH.Models
{
    public class AccountLoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public int TokenTimeout { get; set; }
        public ContextData Context { get; set; } = default!;
        public IEnumerable<LoginDatabaseContext> Databases { get; set; } = Array.Empty<LoginDatabaseContext>();
    }
}

