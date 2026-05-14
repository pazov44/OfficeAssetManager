using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAssetManager.Core.Domain.Enums
{
    public enum AssetStatus
    {
        Available,
        Reserved,
        InUse,
        UnderRepair,
        Retired
    }
}
