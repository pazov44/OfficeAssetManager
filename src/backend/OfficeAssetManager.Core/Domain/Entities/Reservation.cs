using OfficeAssetManager.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAssetManager.Core.Domain.Entities
{
    public class Reservation : BaseEntity
    {
        public int UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int AssetId { get; set; }
        public Asset? Asset { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
        public string? AdminNotes { get; set; }
    }
}
