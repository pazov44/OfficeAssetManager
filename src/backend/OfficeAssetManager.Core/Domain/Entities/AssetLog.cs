using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAssetManager.Core.Domain.Entities
{
    public class AssetLog : BaseEntity
    {
        public int? AssetId { get; set; }
        public Asset? Asset { get; set; }

        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
    }
}