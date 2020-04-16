﻿using System.ComponentModel.DataAnnotations;

namespace Voluntary.App.Models
{
    public class LoginViewModel
    {
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}