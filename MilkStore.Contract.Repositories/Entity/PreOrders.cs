using MilkStore.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Contract.Repositories.Entity
{
    public class PreOrders : BaseEntity
    {
        public Guid UserID { get; set; }
        public Guid ProductID { get; set; }
        public DateTime PreoderDate { get; set; }
        public required string Status { get; set; }
        public required int Quantity { get; set; }
    }
}
