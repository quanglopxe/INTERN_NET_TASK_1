﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkStore.ModelViews.UserModelViews
{
    public class UserModelView
    {
  
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 6)]
        public string? Password { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 6)]
        public string? PasswordHash { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
