﻿using System.ComponentModel.DataAnnotations;

namespace MilkStore.ModelViews.ProductsModelViews
{
    public class ProductsModel
    {
        public string Id {  get; set; }
        [Required(ErrorMessage = "Tên sp không được để trống")]
        public required string ProductName { get; set; }
        [Required(ErrorMessage = "Giá sp không được để trống")]
        public required double Price { get; set; }
        [Required(ErrorMessage = "Mô tả sp không được để trống")]
        public required string Description { get; set; }
        [Required(ErrorMessage = "sl sp không được để trống")]
        public required int QuantityInStock { get; set; }
        public string ImageUrl { get; set; }
        [Required(ErrorMessage = "Id loại không được để trống")]
        public string CategoryId { get; set; }
        //public List<int>? ProductIDs { get; set; }
    }
}