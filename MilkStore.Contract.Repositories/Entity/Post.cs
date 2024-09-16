using MilkStore.Core.Base;
using System.Text.Json.Serialization;

namespace MilkStore.Contract.Repositories.Entity
{
    public class Post : BaseEntity
    {
        public required string Title { get; set; } = string.Empty;
        public required string Content { get; set; } = string.Empty;
        public string? Image { get; set; }

        public virtual ICollection<PostProduct> PostProducts { get; set; } = new List<PostProduct>();

    }
}