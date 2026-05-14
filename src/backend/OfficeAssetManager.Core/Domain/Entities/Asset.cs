using OfficeAssetManager.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAssetManager.Core.Domain.Entities
{
    public class Asset : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string AssetTag { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public AssetStatus Status { get; set; } = AssetStatus.Available;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
