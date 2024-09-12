using MilkStore.Core.Base;

namespace MilkStore.Contract.Repositories.Entity
{
    public class Products : BaseEntity
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int QuantityInStock { get; set; }
        public string ImageUrl { get; set; }

    }
}