using MilkStore.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Contract.Repositories.Entity
{
    public class OrderDetails : BaseEntity
    {
        public string OrderID { get; set; }
        public string ProductID { get; set; }
        public required int Quantity { get; set; }
        public required double UnitPrice { get; set; }
        public double TotalAmount => Quantity * UnitPrice;

        public virtual Order Order { get; set; }
        public virtual Products Products { get; set; }
    }
}
