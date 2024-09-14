using MilkStore.Core.Base;

using System.ComponentModel.DataAnnotations;


namespace MilkStore.Contract.Repositories.Entity
{
    public class User : BaseEntity
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 6)]
        public string PasswordHash { get; set; }

        public string Address { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public int Points { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
