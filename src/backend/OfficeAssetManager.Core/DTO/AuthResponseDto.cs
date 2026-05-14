using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAssetManager.Core.DTO
{
    public class AuthResponseDto
    {
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
    }
}
