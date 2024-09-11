﻿using System.ComponentModel.DataAnnotations;

namespace MilkStore.ModelViews.AuthModelViews
{
    public class LoginModelView
    {
        [Required(ErrorMessage = "Username is required")]
        public required string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }
    }
}
