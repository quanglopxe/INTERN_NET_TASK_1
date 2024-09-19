using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.ModelViews.CategoryModelViews
{
    public class CategoryModel
    {
        public string Id {  get; set; }
        [Required(ErrorMessage = "Tên sp không được để trống")]
        public required string CategoryName { get; set; }
    }
}
