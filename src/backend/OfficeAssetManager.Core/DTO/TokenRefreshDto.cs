using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAssetManager.Core.DTO
{
    public class TokenRequestDto
    {
        //jwt
        public string Token { get; set; } = null!;

        //refresh token
        public string RefreshToken { get; set; } = null!;
    }
}
