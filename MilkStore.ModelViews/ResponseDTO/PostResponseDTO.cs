using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.ModelViews.ResponseDTO
{
    public class PostResponseDTO
    {
        public string PostID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset LastUpdatedAt { get; set; }
        public DateTimeOffset? DeletedTime { get; set; } = null;
        public List<ProductResponseDTO> Products { get; set; } = new List<ProductResponseDTO>();
    }
}
