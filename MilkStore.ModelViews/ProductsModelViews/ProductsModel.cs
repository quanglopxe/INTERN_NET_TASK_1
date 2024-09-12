using System.ComponentModel.DataAnnotations;

namespace MilkStore.ModelViews.ProductsModelViews
{
    public class ProductsModel
    {
        [Required(ErrorMessage = "Tên sp không được để trống")]
        public required string ProductName { get; set; }
        [Required(ErrorMessage = "Giá sp không được để trống")]
        public required decimal Price { get; set; }
        [Required(ErrorMessage = "Mô tả sp không được để trống")]
        public required string Description { get; set; }
        [Required(ErrorMessage = "sl sp không được để trống")]
        public required int QuantityInStock { get; set; }
        public string ImageUrl { get; set; }
        //public List<int>? ProductIDs { get; set; }
    }
}