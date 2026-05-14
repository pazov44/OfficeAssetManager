using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAssetManager.Core.DTO
{
    public class RegisterDto
    {

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string UserName { get; set; } = null!;

    }
}
