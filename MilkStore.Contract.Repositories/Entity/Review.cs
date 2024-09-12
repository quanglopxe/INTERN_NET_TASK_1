using MilkStore.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.Contract.Repositories.Entity
{
    public class Review : BaseEntity
    {
        public Guid ProductID { get; set; }
        public Guid UserID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
