using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAssetManager.Core.Configuration
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int TokenExpirationMinutes { get; set; }
        public int RefreshTokenExpirationDays { get; set; }
        public string SecretKey { get; set; } = string.Empty;
    }
}
