using MilkStore.Core.Base;
using MilkStore.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MilkStore.Contract.Repositories.Entity
{
    public class OrderDetails
    {
        public string OrderID { get; set; }
        public string ProductID { get; set; }
        public required int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double TotalAmount => Quantity * UnitPrice;
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset CreatedTime { get; set; } = CoreHelper.SystemTimeNow;
        public DateTimeOffset LastUpdatedTime { get; set; } = CoreHelper.SystemTimeNow;
        public DateTimeOffset? DeletedTime { get; set; }
        [JsonIgnore]
        public virtual Order Order { get; set; }
        [JsonIgnore]
        public virtual Products Products { get; set; }
    }
}
