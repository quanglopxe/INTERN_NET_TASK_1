using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.ModelViews.PreOrdersModelView
{
    public class PreOrdersModelView
    {
        public required Guid UserID { get; set; }
        public string ProductID { get; set; }        
        public required string Status { get; set; }
        public required int Quantity { get; set; }
    }
}
