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
        public Guid OrderID { get; set; }
        public Guid ProductID { get; set; }
        public required int Quantity { get; set; }
        public required double UnitPrice { get; set; }
        public double TotalAmount => Quantity * UnitPrice;
    }
}
