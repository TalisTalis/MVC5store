using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_magic_store.Models.ViewModels.Account
{
    public class LoginUserVM
    {
        [Required]
        [DisplayName("Login")]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [DisplayName("Remember Me")]
        public bool RememberMe { get; set; }
    }
}