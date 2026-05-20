using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAssetManager.Core.DTO
{
    public class AuthResponseFailDto : AuthResponseDto
    {
        // Automatically ignores this property when it's null (like on successful logins!)
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<string>? Errors { get; set; }
    }
}
